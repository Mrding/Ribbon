#region Header

/**
 * <pre>
 *
 *  Work Force Management
 *  File: StaffingCalculatorModel.cs
 *
 *  Grandsys, Inc.
 *  Copyright (C): 2010
 *
 *  Description:
 *  Init StaffingCalculatorModel
 *
 *  Note
 *  Created By: Administrator at 3/17/2010 11:02:40 AM
 *
 * </pre>
 */

#endregion Header

using System;

namespace Luna.Statistic.Domain.Impl
{
    using System.Linq;
    using System.Collections.Generic;
    using Core.Extensions;
    using Common;
    using Core;
    using Infrastructure.Domain;
    using Shifts.Domain;
    using Data.Repositories;
    using Model;
    using uNhAddIns.Adapters;
    using System.Collections;

    [PersistenceConversational(MethodsIncludeMode = MethodsIncludeMode.Explicit)]
    public class StaffingCalculatorModel : IStaffingCalculatorModel
    {
        private readonly IForecastRepository _forecastRepository;
        private ITerm _enquiryRange; // 排班期扩展范围

        public StaffingCalculatorModel(IForecastRepository forecastRepository)
        {
            _forecastRepository = forecastRepository;
        }

        

        /// <summary>
        /// 获取预测数据
        /// </summary>
        /// <param name="scheduleId">为了获取排班期的SQ</param>
        /// <param name="enquiryRange">通常为排班期范围</param>
        /// <param name="convertTo">UI呈现属性布置,line的颜色和描述</param>
        /// <returns></returns>
        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public IServiceQueueContainer Preparing(object scheduleId, ITerm enquiryRange, System.Func<double[], int, string, object, IVisibleLinerData> convertTo)
        {
            _enquiryRange = enquiryRange;
            var schedule = _forecastRepository.Get<Schedule>(scheduleId);
            var serviceQueues = schedule.ServiceQueues.Keys.ToArray();
            var set = new ServiceQueueContainer(serviceQueues, enquiryRange, convertTo);

            
            _forecastRepository.LoadForecastRaw(serviceQueues, enquiryRange.Start, enquiryRange.End, t =>
            {
                set[t.GroupBy<ServiceQueue>().GetHashCode()].Concat(t);
            });

            set.CalculateForecastStatistics(schedule.Shrinkages.Select(o => o.AsArray()).ToArray(), (int)enquiryRange.Start.DayOfWeek);
            return set;
        }


        private Dictionary<IServiceQueueStatistic, double> AgentSkillToServiceQeueue(IAgent agent, IServiceQueueContainer serviceQueueContainer)
        {
            var mapping = new Dictionary<IServiceQueueStatistic, double>();

            foreach (var skill in agent.Profile.Skills)
            {
                var q = serviceQueueContainer[skill.Key];
                if (q != null)
                    mapping.Add(q, skill.Value);
            }
            return mapping;
        }

        private Dictionary<IAgent, Dictionary<IServiceQueueStatistic, double>> ConvertAgentSkillToServiceQeueue(IEnumerable agentsSource,
           IServiceQueueContainer serviceQueueContainer)
        {
            var agents = new Dictionary<IAgent, Dictionary<IServiceQueueStatistic, double>>();
            foreach (IAgent agent in agentsSource)
            {
                var mapping = AgentSkillToServiceQeueue(agent, serviceQueueContainer);

                if (0 < mapping.Count)// 技能未能夠匹配到SQ
                    agents[agent] = mapping;

                //agents[agent] = new Dictionary<IServiceQueueStatistic, double>();
                //foreach (var skill in agent.Profile.Skills)
                //{
                //    var q = serviceQueueContainer[skill.Key];
                //    if (q != null)
                //        agents[agent].Add(q, skill.Value);
                //}
                //if (0 < agents[agent].Count) 
                //{
                //   //x agent.BuildOnlines(_enquiryRange.Start, _enquiryRange.End);// 注意:性能消耗
                //}
                //else
                //    agents.Remove(agent);
            }
            return agents;
        }

        //[PersistenceConversation(Exclude = true)]
        //public void Fetch(int[] startPositions, int length, IAgent agent)
        //{
        //    AgentSkillToServiceQeueue(agent,)
        //}

        [PersistenceConversation(Exclude = true)]
        public void Fetch(int start, int end, IServiceQueueContainer x, IEnumerable agentsSource)
        {
            var agents = ConvertAgentSkillToServiceQeueue(agentsSource, x);
            // var sumOfCvs = new double[end - start/96 + 1];

            for (var i = start; i < end; i++)
            {
                var index = i;
                var groups = new Dictionary<string, Group>();
                var queueStaffings = new Dictionary<IServiceQueueStatistic, double>();
                var queues = new List<IServiceQueueStatistic>();

                //grouping
                foreach (var item in agents)
                {
                    var isOnline = index.DvideTo5Min(item.Key.Onlines);
                    item.Value.Distribute((q, productivity) =>
                    {
                        var noForecasting = q.ForceastStaffing[index] <= 0;

                        if (!queueStaffings.ContainsKey(q))
                        {
                            // unique operation, do once
                            q.Reset(index);
                            queues.Add(q);
                            queueStaffings[q] = noForecasting ? 0.1d : q.ForceastStaffing[index]; // atention
                        }

                        q.AssignedMaxStaffing[index] += isOnline ? productivity : 0;
                        return isOnline;
                    }, ref groups);
                }

                if (groups.Count != 0)
                {
                    Dictionary<int, Tuple<IServiceQueueStatistic, double>> vEx;
                    double[,] dMtx;
                    double[] rhs;
                    int mtxCurrentIndex;

                    LinearAlgebraLib.Build(groups.Values, out mtxCurrentIndex, out dMtx, out rhs, out vEx);
                    LinearAlgebraLib.RatioX(queues, queueStaffings, ref mtxCurrentIndex, ref dMtx, vEx);

                    int info;
                    double[] result;

                    SimplexStaffing.FS(dMtx, groups.Count, rhs, out info, out result);

                    for (var j = 0; j < vEx.Count; j++)
                        vEx[j].Item1.AssignedStaffing[index] += result[j]; // staffing contribution result
                }

                x.CalculateAssignedStatistics(index, groups.Count == 0 ? 0d : default(double?));
            }
        }

        [PersistenceConversation(ConversationEndMode = EndMode.Continue)]
        public void Release()
        {
            _forecastRepository.Clear();
        }
    }
}

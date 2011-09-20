using System;
using Luna.Infrastructure.Domain;
using Luna.Common;
using System.Collections.Generic;
using Luna.Shifts.Domain;
using System.Collections;

namespace Luna.Statistic.Domain.Model
{
    public interface IStaffingCalculatorModel
    {
        /// <summary>
        /// 读取预测数据
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="enquiryRange"></param>
        /// <param name="itemContstruction">建立数据模型和UI有关系</param>
        /// <returns></returns>
        IServiceQueueContainer Preparing(object scheduleId, ITerm enquiryRange, Func<double[], int, string, object, IVisibleLinerData> itemContstruction);

        void Fetch(int start, int end, IServiceQueueContainer x, IEnumerable agentsSource);

        void Release();
    }
}

using System;

namespace Luna.Shifts.Domain.Model
{
    public class SeatingEngineStatus : EventArgs
    {
        public SeatingEngineStatus()
        {
            Stage2ArrangedAmount = new int[3];
            Stage2ArrangedPercentage = new double[3];
        }

        private string _textMessage;

        /// <summary>
        /// 文字輸出
        /// </summary>
        public virtual string TextMessage
        {
            get { return _textMessage; }
            set { _textMessage = value;
                
            }
        }

        /// <summary>
        /// 參與排座人員總數
        /// </summary>
        public virtual int EmployeeAmount { get; set; }
        /// <summary>
        /// 總共Assignment數量
        /// </summary>
        public virtual int AssignmentAmount { get; set; }
        /// <summary>
        /// 總共SeatArrangement數量
        /// </summary>
        public virtual int SeatArrangementAmount { get; set; }
        /// <summary>
        /// 進度[0,1]
        /// </summary>
        public virtual double Process { get; set; }

        /// <summary>
        /// 總合連續Assignemnt數量 , 百分比
        /// </summary>
        public virtual int TotalContinueAssignmentAmount { get; set; }
        public virtual double TotalContinueAssignmentPercentage { get; set; }

        /// <summary>
        /// 總合安排完成SeatArrangement數量 , 百分比
        /// </summary>
        public virtual int ArrangedSeatArrangementAmount { get; set; }
        public virtual double ArrangedSeatArrangementPercentage { get; set; }

        /// <summary>
        /// 總合未安排SeatArrangement數量 , 百分比
        /// </summary>
        public virtual int UnArrangedSeatArrangementAmount { get; set; }
        public virtual double UnArrangedSeatArrangementPercentage { get; set; }


        /// <summary>
        /// 時段集中安排SeatArrangement數量 , 百分比
        /// </summary>
        public virtual int Stage1ArrangedAmount { get; set; }
        public virtual double Stage1ArrangePercentage { get; set; }

        /// <summary>
        /// 第[1~3]優先人員安排SeatArrangement數量 , 百分比
        /// </summary>
        public virtual int[] Stage2ArrangedAmount { get; set; }
        public virtual double[] Stage2ArrangedPercentage { get; set; }

        /// <summary>
        /// 優先組織安排SeatArrangement數量 , 百分比
        /// </summary>
        public virtual int Stage3ArrangedAmount { get; set; }
        public virtual double Stage3ArrangePercentage { get; set; }

        /// <summary>
        /// 話房組織安排SeatArrangement數量 , 百分比
        /// </summary>
        public virtual int Stage4ArrangedAmount { get; set; }
        public virtual double Stage4ArrangePercentage { get; set; }

        /// <summary>
        /// 席位利用率
        /// </summary>
        public virtual double SeatUsageRate { get; set; }
    }
}

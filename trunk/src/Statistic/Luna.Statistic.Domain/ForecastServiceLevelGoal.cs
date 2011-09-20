using System;
using Luna.Infrastructure.Domain;

namespace Luna.Statistic.Domain
{
    [System.Diagnostics.DebuggerDisplay("{Date}")]
    public class ForecastServiceLevelGoal : IDailyObject
    {
        private double[] _sls;

        public virtual double[] Goals
        {
            get
            {
                return _sls ??
                       (_sls =
                        new double[]
                            {
                                SL0, SL1, SL2, SL3, SL4, SL5, SL6, SL7, SL8, SL9, SL10, SL11, SL12, SL13, SL14, SL15, SL16,
                                SL17, SL18, SL19, SL20, SL21, SL22, SL23
                            });
            }
            set
            {
                SL0 = (int)value[0];
                SL1 = (int)value[1];
                SL2 = (int)value[2];
                SL3 = (int)value[3];
                SL4 = (int)value[4];
                SL5 = (int)value[5];
                SL6 = (int)value[6];
                SL7 = (int)value[7];
                SL8 = (int)value[8];
                SL9 = (int)value[9];
                SL10 = (int)value[10];
                SL11 = (int)value[11];
                SL12 = (int)value[12];
                SL13 = (int)value[13];
                SL14 = (int)value[14];
                SL15 = (int)value[15];
                SL16 = (int)value[16];
                SL17 = (int)value[17];
                SL18 = (int)value[18];
                SL19 = (int)value[19];
                SL20 = (int)value[20];
                SL21 = (int)value[21];
                SL22 = (int)value[22];
                SL23 = (int)value[23];
                _sls = value;
            }
        }

        public virtual DateTime Date { get; set; }

        public virtual T GroupBy<T>() where T : class 
        {
            return Forecast.ServiceQueue as T;
        }

        #region NHibernate Properties

        public virtual Forecast Forecast { get; set; }

        protected int SL0;
        protected int SL1;
        protected int SL2;
        protected int SL3;
        protected int SL4;
        protected int SL5;
        protected int SL6;
        protected int SL7;
        protected int SL8;
        protected int SL9;
        protected int SL10;
        protected int SL11;
        protected int SL12;
        protected int SL13;
        protected int SL14;
        protected int SL15;
        protected int SL16;
        protected int SL17;
        protected int SL18;
        protected int SL19;
        protected int SL20;
        protected int SL21;
        protected int SL22;
        protected int SL23;

        #endregion

    }
}
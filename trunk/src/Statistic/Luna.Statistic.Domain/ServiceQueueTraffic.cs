using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Luna.Infrastructure.Domain;

namespace Luna.Statistic.Domain
{
    [System.Diagnostics.DebuggerDisplay("{Date}")]
    public class ServiceQueueTraffic : IDailyObject
    {
        private double[] _cvs;

        public virtual ServiceQueue ServiceQueue { get; set; }

        public virtual DateTime Date { get; set; }


        
        public virtual double[] CVs
        {
            get
            {
                if (_cvs == null)
                {
                    _cvs = new[]
                                  {
                                      V0,
                                      V1,
                                      V2,
                                      V3,
                                      V4,
                                      V5,
                                      V6,
                                      V7,
                                      V8,
                                      V9,
                                      V10,
                                      V11,
                                      V12,
                                      V13,
                                      V14,
                                      V15,
                                      V16,
                                      V17,
                                      V18,
                                      V19,
                                      V20,
                                      V21,
                                      V22,
                                      V23,
                                      V24,
                                      V25,
                                      V26,
                                      V27,
                                      V28,
                                      V29,
                                      V30,
                                      V31,
                                      V32,
                                      V33,
                                      V34,
                                      V35,
                                      V36,
                                      V37,
                                      V38,
                                      V39,
                                      V40,
                                      V41,
                                      V42,
                                      V43,
                                      V44,
                                      V45,
                                      V46,
                                      V47,
                                      V48,
                                      V49,
                                      V50,
                                      V51,
                                      V52,
                                      V53,
                                      V54,
                                      V55,
                                      V56,
                                      V57,
                                      V58,
                                      V59,
                                      V60,
                                      V61,
                                      V62,
                                      V63,
                                      V64,
                                      V65,
                                      V66,
                                      V67,
                                      V68,
                                      V69,
                                      V70,
                                      V71,
                                      V72,
                                      V73,
                                      V74,
                                      V75,
                                      V76,
                                      V77,
                                      V78,
                                      V79,
                                      V80,
                                      V81,
                                      V82,
                                      V83,
                                      V84,
                                      V85,
                                      V86,
                                      V87,
                                      V88,
                                      V89,
                                      V90,
                                      V91,
                                      V92,
                                      V93,
                                      V94,
                                      V95
                                  };
                }
                return _cvs;
            }
        }


        private ReadOnlyCollection<double> _sls;
        public virtual IList<double> SLs
        {
            get
            {
                if (_sls == null)
                {
                    var ahts = new List<double>
                                   {
                                       S0,
                                       S1,
                                       S2,
                                       S3,
                                       S4,
                                       S5,
                                       S6,
                                       S7,
                                       S8,
                                       S9,
                                       S10,
                                       S11,
                                       S12,
                                       S13,
                                       S14,
                                       S15,
                                       S16,
                                       S17,
                                       S18,
                                       S19,
                                       S20,
                                       S21,
                                       S22,
                                       S23,
                                       S24,
                                       S25,
                                       S26,
                                       S27,
                                       S28,
                                       S29,
                                       S30,
                                       S31,
                                       S32,
                                       S33,
                                       S34,
                                       S35,
                                       S36,
                                       S37,
                                       S38,
                                       S39,
                                       S40,
                                       S41,
                                       S42,
                                       S43,
                                       S44,
                                       S45,
                                       S46,
                                       S47,
                                       S48,
                                       S49,
                                       S50,
                                       S51,
                                       S52,
                                       S53,
                                       S54,
                                       S55,
                                       S56,
                                       S57,
                                       S58,
                                       S59,
                                       S60,
                                       S61,
                                       S62,
                                       S63,
                                       S64,
                                       S65,
                                       S66,
                                       S67,
                                       S68,
                                       S69,
                                       S70,
                                       S71,
                                       S72,
                                       S73,
                                       S74,
                                       S75,
                                       S76,
                                       S77,
                                       S78,
                                       S79,
                                       S80,
                                       S81,
                                       S82,
                                       S83,
                                       S84,
                                       S85,
                                       S86,
                                       S87,
                                       S88,
                                       S89,
                                       S90,
                                       S91,
                                       S92,
                                       S93,
                                       S94,
                                       S95
                                   };
                    _sls = ahts.AsReadOnly();
                }
                return _sls;
            }
        }

        public virtual T GroupBy<T>() where T : class
        {
            return ServiceQueue as T;
        }

        #region NHibernate Properties

        //CallVolume用V简写、ServiceLevel用S简写、AHT用H简写、AbandonedCall用A简写、MaxStaff用M简写

        protected double V0 { get; set; }
        protected double V1 { get; set; }
        protected double V2 { get; set; }
        protected double V3 { get; set; }
        protected double V4 { get; set; }
        protected double V5 { get; set; }
        protected double V6 { get; set; }
        protected double V7 { get; set; }
        protected double V8 { get; set; }
        protected double V9 { get; set; }
        protected double V10 { get; set; }
        protected double V11 { get; set; }
        protected double V12 { get; set; }
        protected double V13 { get; set; }
        protected double V14 { get; set; }
        protected double V15 { get; set; }
        protected double V16 { get; set; }
        protected double V17 { get; set; }
        protected double V18 { get; set; }
        protected double V19 { get; set; }
        protected double V20 { get; set; }
        protected double V21 { get; set; }
        protected double V22 { get; set; }
        protected double V23 { get; set; }
        protected double V24 { get; set; }
        protected double V25 { get; set; }
        protected double V26 { get; set; }
        protected double V27 { get; set; }
        protected double V28 { get; set; }
        protected double V29 { get; set; }
        protected double V30 { get; set; }
        protected double V31 { get; set; }
        protected double V32 { get; set; }
        protected double V33 { get; set; }
        protected double V34 { get; set; }
        protected double V35 { get; set; }
        protected double V36 { get; set; }
        protected double V37 { get; set; }
        protected double V38 { get; set; }
        protected double V39 { get; set; }
        protected double V40 { get; set; }
        protected double V41 { get; set; }
        protected double V42 { get; set; }
        protected double V43 { get; set; }
        protected double V44 { get; set; }
        protected double V45 { get; set; }
        protected double V46 { get; set; }
        protected double V47 { get; set; }
        protected double V48 { get; set; }
        protected double V49 { get; set; }
        protected double V50 { get; set; }
        protected double V51 { get; set; }
        protected double V52 { get; set; }
        protected double V53 { get; set; }
        protected double V54 { get; set; }
        protected double V55 { get; set; }
        protected double V56 { get; set; }
        protected double V57 { get; set; }
        protected double V58 { get; set; }
        protected double V59 { get; set; }
        protected double V60 { get; set; }
        protected double V61 { get; set; }
        protected double V62 { get; set; }
        protected double V63 { get; set; }
        protected double V64 { get; set; }
        protected double V65 { get; set; }
        protected double V66 { get; set; }
        protected double V67 { get; set; }
        protected double V68 { get; set; }
        protected double V69 { get; set; }
        protected double V70 { get; set; }
        protected double V71 { get; set; }
        protected double V72 { get; set; }
        protected double V73 { get; set; }
        protected double V74 { get; set; }
        protected double V75 { get; set; }
        protected double V76 { get; set; }
        protected double V77 { get; set; }
        protected double V78 { get; set; }
        protected double V79 { get; set; }
        protected double V80 { get; set; }
        protected double V81 { get; set; }
        protected double V82 { get; set; }
        protected double V83 { get; set; }
        protected double V84 { get; set; }
        protected double V85 { get; set; }
        protected double V86 { get; set; }
        protected double V87 { get; set; }
        protected double V88 { get; set; }
        protected double V89 { get; set; }
        protected double V90 { get; set; }
        protected double V91 { get; set; }
        protected double V92 { get; set; }
        protected double V93 { get; set; }
        protected double V94 { get; set; }
        protected double V95 { get; set; }

        protected double S0 { get; set; }
        protected double S1 { get; set; }
        protected double S2 { get; set; }
        protected double S3 { get; set; }
        protected double S4 { get; set; }
        protected double S5 { get; set; }
        protected double S6 { get; set; }
        protected double S7 { get; set; }
        protected double S8 { get; set; }
        protected double S9 { get; set; }
        protected double S10 { get; set; }
        protected double S11 { get; set; }
        protected double S12 { get; set; }
        protected double S13 { get; set; }
        protected double S14 { get; set; }
        protected double S15 { get; set; }
        protected double S16 { get; set; }
        protected double S17 { get; set; }
        protected double S18 { get; set; }
        protected double S19 { get; set; }
        protected double S20 { get; set; }
        protected double S21 { get; set; }
        protected double S22 { get; set; }
        protected double S23 { get; set; }
        protected double S24 { get; set; }
        protected double S25 { get; set; }
        protected double S26 { get; set; }
        protected double S27 { get; set; }
        protected double S28 { get; set; }
        protected double S29 { get; set; }
        protected double S30 { get; set; }
        protected double S31 { get; set; }
        protected double S32 { get; set; }
        protected double S33 { get; set; }
        protected double S34 { get; set; }
        protected double S35 { get; set; }
        protected double S36 { get; set; }
        protected double S37 { get; set; }
        protected double S38 { get; set; }
        protected double S39 { get; set; }
        protected double S40 { get; set; }
        protected double S41 { get; set; }
        protected double S42 { get; set; }
        protected double S43 { get; set; }
        protected double S44 { get; set; }
        protected double S45 { get; set; }
        protected double S46 { get; set; }
        protected double S47 { get; set; }
        protected double S48 { get; set; }
        protected double S49 { get; set; }
        protected double S50 { get; set; }
        protected double S51 { get; set; }
        protected double S52 { get; set; }
        protected double S53 { get; set; }
        protected double S54 { get; set; }
        protected double S55 { get; set; }
        protected double S56 { get; set; }
        protected double S57 { get; set; }
        protected double S58 { get; set; }
        protected double S59 { get; set; }
        protected double S60 { get; set; }
        protected double S61 { get; set; }
        protected double S62 { get; set; }
        protected double S63 { get; set; }
        protected double S64 { get; set; }
        protected double S65 { get; set; }
        protected double S66 { get; set; }
        protected double S67 { get; set; }
        protected double S68 { get; set; }
        protected double S69 { get; set; }
        protected double S70 { get; set; }
        protected double S71 { get; set; }
        protected double S72 { get; set; }
        protected double S73 { get; set; }
        protected double S74 { get; set; }
        protected double S75 { get; set; }
        protected double S76 { get; set; }
        protected double S77 { get; set; }
        protected double S78 { get; set; }
        protected double S79 { get; set; }
        protected double S80 { get; set; }
        protected double S81 { get; set; }
        protected double S82 { get; set; }
        protected double S83 { get; set; }
        protected double S84 { get; set; }
        protected double S85 { get; set; }
        protected double S86 { get; set; }
        protected double S87 { get; set; }
        protected double S88 { get; set; }
        protected double S89 { get; set; }
        protected double S90 { get; set; }
        protected double S91 { get; set; }
        protected double S92 { get; set; }
        protected double S93 { get; set; }
        protected double S94 { get; set; }
        protected double S95 { get; set; }

        #endregion
    }
}

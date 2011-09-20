using System;


namespace Luna.Statistic.Domain
{
    [System.Diagnostics.DebuggerDisplay("{Date}")]
    public class ForecastTraffic : IDailyObject
    {
        private double[] _ahts;
        private double[] _cvs;
        
        private double[] _serviceTotalSeconds;

        public virtual DateTime Date { get; set; }



        public virtual T GroupBy<T>() where T : class
        {
            return Forecast.ServiceQueue as T;
        }

        public virtual double SumOfCVs
        {
            get
            {
                return CV1 + CV2 + CV3 + CV4 + CV5 + CV6 + CV7 + CV8 + CV9 + CV10 + CV11 + CV12 + CV13 + CV14 + CV15 +
                       CV16 + CV17
                       + CV18 + CV19 + CV20 + CV21 + CV22 + CV23 + CV24 + CV25 + CV26 + CV27 + CV28 + CV29 + CV30 + CV31 +
                       CV32 +
                       CV33 + CV34 + CV35 + CV36 + CV37 + CV38 + CV39 + CV40 + CV41 + CV42 + CV43 + CV44 + CV45 + CV46 +
                       CV47 +
                       CV48 + CV49 + CV50 + CV51 + CV52 + CV53 + CV54 + CV55 + CV56 + CV57 + CV58 + CV59 + CV60 + CV61 +
                       CV62 +
                       CV63 + CV64 + CV65 + CV66 + CV67 + CV68 + CV69 + CV70 + CV71 + CV72 + CV73 + CV74 + CV75 + CV76 +
                       CV77 +
                       CV78 + CV79 + CV80 + CV81 + CV82 + CV83 + CV84 + CV85 + CV86 + CV87 + CV88 + CV89 + CV90 + CV91 +
                       CV92 +
                       CV93 + CV94 + CV95 + CV96
                    ;
            }
        }

        public virtual double[] CVs
        {
            get
            {
                return _cvs ??
                       (_cvs =
                        new double[]
                            {
                                CV1, CV2, CV3, CV4, CV5, CV6, CV7, CV8, CV9, CV10, CV11, CV12, CV13, CV14, CV15, CV16, CV17
                                , CV18, CV19, CV20, CV21, CV22, CV23, CV24, CV25, CV26, CV27, CV28, CV29, CV30, CV31, CV32,
                                CV33, CV34, CV35, CV36, CV37, CV38, CV39, CV40, CV41, CV42, CV43, CV44, CV45, CV46, CV47,
                                CV48, CV49, CV50, CV51, CV52, CV53, CV54, CV55, CV56, CV57, CV58, CV59, CV60, CV61, CV62,
                                CV63, CV64, CV65, CV66, CV67, CV68, CV69, CV70, CV71, CV72, CV73, CV74, CV75, CV76, CV77,
                                CV78, CV79, CV80, CV81, CV82, CV83, CV84, CV85, CV86, CV87, CV88, CV89, CV90, CV91, CV92,
                                CV93, CV94, CV95, CV96
                            });
            }
            set
            {
                CV1 = (int)value[0];
                CV2 = (int)value[1];
                CV3 = (int)value[2];
                CV4 = (int)value[3];
                CV5 = (int)value[4];
                CV6 = (int)value[5];
                CV7 = (int)value[6];
                CV8 = (int)value[7];
                CV9 = (int)value[8];
                CV10 = (int)value[9];
                CV11 = (int)value[10];
                CV12 = (int)value[11];
                CV13 = (int)value[12];
                CV14 = (int)value[13];
                CV15 = (int)value[14];
                CV16 = (int)value[15];
                CV17 = (int)value[16];
                CV18 = (int)value[17];
                CV19 = (int)value[18];
                CV20 = (int)value[19];
                CV21 = (int)value[20];
                CV22 = (int)value[21];
                CV23 = (int)value[22];
                CV24 = (int)value[23];
                CV25 = (int)value[24];
                CV26 = (int)value[25];
                CV27 = (int)value[26];
                CV28 = (int)value[27];
                CV29 = (int)value[28];
                CV30 = (int)value[29];
                CV31 = (int)value[30];
                CV32 = (int)value[31];
                CV33 = (int)value[32];
                CV34 = (int)value[33];
                CV35 = (int)value[34];
                CV36 = (int)value[35];
                CV37 = (int)value[36];
                CV38 = (int)value[37];
                CV39 = (int)value[38];
                CV40 = (int)value[39];
                CV41 = (int)value[40];
                CV42 = (int)value[41];
                CV43 = (int)value[42];
                CV44 = (int)value[43];
                CV45 = (int)value[44];
                CV46 = (int)value[45];
                CV47 = (int)value[46];
                CV48 = (int)value[47];
                CV49 = (int)value[48];
                CV50 = (int)value[49];
                CV51 = (int)value[50];
                CV52 = (int)value[51];
                CV53 = (int)value[52];
                CV54 = (int)value[53];
                CV55 = (int)value[54];
                CV56 = (int)value[55];
                CV57 = (int)value[56];
                CV58 = (int)value[57];
                CV59 = (int)value[58];
                CV60 = (int)value[59];
                CV61 = (int)value[60];
                CV62 = (int)value[61];
                CV63 = (int)value[62];
                CV64 = (int)value[63];
                CV65 = (int)value[64];
                CV66 = (int)value[65];
                CV67 = (int)value[66];
                CV68 = (int)value[67];
                CV69 = (int)value[68];
                CV70 = (int)value[69];
                CV71 = (int)value[70];
                CV72 = (int)value[71];
                CV73 = (int)value[72];
                CV74 = (int)value[73];
                CV75 = (int)value[74];
                CV76 = (int)value[75];
                CV77 = (int)value[76];
                CV78 = (int)value[77];
                CV79 = (int)value[78];
                CV80 = (int)value[79];
                CV81 = (int)value[80];
                CV82 = (int)value[81];
                CV83 = (int)value[82];
                CV84 = (int)value[83];
                CV85 = (int)value[84];
                CV86 = (int)value[85];
                CV87 = (int)value[86];
                CV88 = (int)value[87];
                CV89 = (int)value[88];
                CV90 = (int)value[89];
                CV91 = (int)value[90];
                CV92 = (int)value[91];
                CV93 = (int)value[92];
                CV94 = (int)value[93];
                CV95 = (int)value[94];
                CV96 = (int)value[95];
                _cvs = value;
            }
        }

        public virtual double[] AHTs
        {
            get
            {
                return _ahts ??
                       (_ahts =
                        new double[]
                            {
                                AHT1, AHT2, AHT3, AHT4, AHT5, AHT6, AHT7, AHT8, AHT9, AHT10, AHT11, AHT12, AHT13, AHT14,
                                AHT15, AHT16, AHT17, AHT18, AHT19, AHT20, AHT21, AHT22, AHT23, AHT24, AHT25, AHT26, AHT27,
                                AHT28, AHT29, AHT30, AHT31, AHT32, AHT33, AHT34, AHT35, AHT36, AHT37, AHT38, AHT39, AHT40,
                                AHT41, AHT42, AHT43, AHT44, AHT45, AHT46, AHT47, AHT48, AHT49, AHT50, AHT51, AHT52, AHT53,
                                AHT54, AHT55, AHT56, AHT57, AHT58, AHT59, AHT60, AHT61, AHT62, AHT63, AHT64, AHT65, AHT66,
                                AHT67, AHT68, AHT69, AHT70, AHT71, AHT72, AHT73, AHT74, AHT75, AHT76, AHT77, AHT78, AHT79,
                                AHT80, AHT81, AHT82, AHT83, AHT84, AHT85, AHT86, AHT87, AHT88, AHT89, AHT90, AHT91, AHT92,
                                AHT93, AHT94, AHT95, AHT96
                            });
            }
            set
            {
                AHT1 = (int)value[0];
                AHT2 = (int)value[1];
                AHT3 = (int)value[2];
                AHT4 = (int)value[3];
                AHT5 = (int)value[4];
                AHT6 = (int)value[5];
                AHT7 = (int)value[6];
                AHT8 = (int)value[7];
                AHT9 = (int)value[8];
                AHT10 = (int)value[9];
                AHT11 = (int)value[10];
                AHT12 = (int)value[11];
                AHT13 = (int)value[12];
                AHT14 = (int)value[13];
                AHT15 = (int)value[14];
                AHT16 = (int)value[15];
                AHT17 = (int)value[16];
                AHT18 = (int)value[17];
                AHT19 = (int)value[18];
                AHT20 = (int)value[19];
                AHT21 = (int)value[20];
                AHT22 = (int)value[21];
                AHT23 = (int)value[22];
                AHT24 = (int)value[23];
                AHT25 = (int)value[24];
                AHT26 = (int)value[25];
                AHT27 = (int)value[26];
                AHT28 = (int)value[27];
                AHT29 = (int)value[28];
                AHT30 = (int)value[29];
                AHT31 = (int)value[30];
                AHT32 = (int)value[31];
                AHT33 = (int)value[32];
                AHT34 = (int)value[33];
                AHT35 = (int)value[34];
                AHT36 = (int)value[35];
                AHT37 = (int)value[36];
                AHT38 = (int)value[37];
                AHT39 = (int)value[38];
                AHT40 = (int)value[39];
                AHT41 = (int)value[40];
                AHT42 = (int)value[41];
                AHT43 = (int)value[42];
                AHT44 = (int)value[43];
                AHT45 = (int)value[44];
                AHT46 = (int)value[45];
                AHT47 = (int)value[46];
                AHT48 = (int)value[47];
                AHT49 = (int)value[48];
                AHT50 = (int)value[49];
                AHT51 = (int)value[50];
                AHT52 = (int)value[51];
                AHT53 = (int)value[52];
                AHT54 = (int)value[53];
                AHT55 = (int)value[54];
                AHT56 = (int)value[55];
                AHT57 = (int)value[56];
                AHT58 = (int)value[57];
                AHT59 = (int)value[58];
                AHT60 = (int)value[59];
                AHT61 = (int)value[60];
                AHT62 = (int)value[61];
                AHT63 = (int)value[62];
                AHT64 = (int)value[63];
                AHT65 = (int)value[64];
                AHT66 = (int)value[65];
                AHT67 = (int)value[66];
                AHT68 = (int)value[67];
                AHT69 = (int)value[68];
                AHT70 = (int)value[69];
                AHT71 = (int)value[70];
                AHT72 = (int)value[71];
                AHT73 = (int)value[72];
                AHT74 = (int)value[73];
                AHT75 = (int)value[74];
                AHT76 = (int)value[75];
                AHT77 = (int)value[76];
                AHT78 = (int)value[77];
                AHT79 = (int)value[78];
                AHT80 = (int)value[79];
                AHT81 = (int)value[80];
                AHT82 = (int)value[81];
                AHT83 = (int)value[82];
                AHT84 = (int)value[83];
                AHT85 = (int)value[84];
                AHT86 = (int)value[85];
                AHT87 = (int)value[86];
                AHT88 = (int)value[87];
                AHT89 = (int)value[88];
                AHT90 = (int)value[89];
                AHT91 = (int)value[90];
                AHT92 = (int)value[91];
                AHT93 = (int)value[92];
                AHT94 = (int)value[93];
                AHT95 = (int)value[94];
                AHT96 = (int)value[95];
                _ahts = value;
            }
        }

        public virtual double[] GetServiceTotalSeconds()
        {
            return _serviceTotalSeconds ??
                      (_serviceTotalSeconds =
                       new double[]
                            {
                               CV1*AHT1,
                                CV2*AHT2,
                                CV3*AHT3,
                                CV4*AHT4,
                                CV5*AHT5,
                                CV6*AHT6,
                                CV7*AHT7,
                                CV8*AHT8,
                                CV9*AHT9,
                                CV10*AHT10,
                                CV11*AHT11,
                                CV12*AHT12,
                                CV13*AHT13,
                                CV14*AHT14,
                                CV15*AHT15,
                                CV16*AHT16,
                                CV17*AHT17,
                                CV18*AHT18,
                                CV19*AHT19,
                                CV20*AHT20,
                                CV21*AHT21,
                                CV22*AHT22,
                                CV23*AHT23,
                                CV24*AHT24,
                                CV25*AHT25,
                                CV26*AHT26,
                                CV27*AHT27,
                                CV28*AHT28,
                                CV29*AHT29,
                                CV30*AHT30,
                                CV31*AHT31,
                                CV32*AHT32,
                                CV33*AHT33,
                                CV34*AHT34,
                                CV35*AHT35,
                                CV36*AHT36,
                                CV37*AHT37,
                                CV38*AHT38,
                                CV39*AHT39,
                                CV40*AHT40,
                                CV41*AHT41,
                                CV42*AHT42,
                                CV43*AHT43,
                                CV44*AHT44,
                                CV45*AHT45,
                                CV46*AHT46,
                                CV47*AHT47,
                                CV48*AHT48,
                                CV49*AHT49,
                                CV50*AHT50,
                                CV51*AHT51,
                                CV52*AHT52,
                                CV53*AHT53,
                                CV54*AHT54,
                                CV55*AHT55,
                                CV56*AHT56,
                                CV57*AHT57,
                                CV58*AHT58,
                                CV59*AHT59,
                                CV60*AHT60,
                                CV61*AHT61,
                                CV62*AHT62,
                                CV63*AHT63,
                                CV64*AHT64,
                                CV65*AHT65,
                                CV66*AHT66,
                                CV67*AHT67,
                                CV68*AHT68,
                                CV69*AHT69,
                                CV70*AHT70,
                                CV71*AHT71,
                                CV72*AHT72,
                                CV73*AHT73,
                                CV74*AHT74,
                                CV75*AHT75,
                                CV76*AHT76,
                                CV77*AHT77,
                                CV78*AHT78,
                                CV79*AHT79,
                                CV80*AHT80,
                                CV81*AHT81,
                                CV82*AHT82,
                                CV83*AHT83,
                                CV84*AHT84,
                                CV85*AHT85,
                                CV86*AHT86,
                                CV87*AHT87,
                                CV88*AHT88,
                                CV89*AHT89,
                                CV90*AHT90,
                                CV91*AHT91,
                                CV92*AHT92,
                                CV93*AHT93,
                                CV94*AHT94,
                                CV95*AHT95,
                                CV96*AHT96
                            });
        }

        #region NHibernate Properties

        public virtual Forecast Forecast { get; set; }

        protected int CV1;
        protected int CV2;
        protected int CV3;
        protected int CV4;
        protected int CV5;
        protected int CV6;
        protected int CV7;
        protected int CV8;
        protected int CV9;
        protected int CV10;
        protected int CV11;
        protected int CV12;
        protected int CV13;
        protected int CV14;
        protected int CV15;
        protected int CV16;
        protected int CV17;
        protected int CV18;
        protected int CV19;
        protected int CV20;
        protected int CV21;
        protected int CV22;
        protected int CV23;
        protected int CV24;
        protected int CV25;
        protected int CV26;
        protected int CV27;
        protected int CV28;
        protected int CV29;
        protected int CV30;
        protected int CV31;
        protected int CV32;
        protected int CV33;
        protected int CV34;
        protected int CV35;
        protected int CV36;
        protected int CV37;
        protected int CV38;
        protected int CV39;
        protected int CV40;
        protected int CV41;
        protected int CV42;
        protected int CV43;
        protected int CV44;
        protected int CV45;
        protected int CV46;
        protected int CV47;
        protected int CV48;
        protected int CV49;
        protected int CV50;
        protected int CV51;
        protected int CV52;
        protected int CV53;
        protected int CV54;
        protected int CV55;
        protected int CV56;
        protected int CV57;
        protected int CV58;
        protected int CV59;
        protected int CV60;
        protected int CV61;
        protected int CV62;
        protected int CV63;
        protected int CV64;
        protected int CV65;
        protected int CV66;
        protected int CV67;
        protected int CV68;
        protected int CV69;
        protected int CV70;
        protected int CV71;
        protected int CV72;
        protected int CV73;
        protected int CV74;
        protected int CV75;
        protected int CV76;
        protected int CV77;
        protected int CV78;
        protected int CV79;
        protected int CV80;
        protected int CV81;
        protected int CV82;
        protected int CV83;
        protected int CV84;
        protected int CV85;
        protected int CV86;
        protected int CV87;
        protected int CV88;
        protected int CV89;
        protected int CV90;
        protected int CV91;
        protected int CV92;
        protected int CV93;
        protected int CV94;
        protected int CV95;
        protected int CV96;

        protected int AHT1;
        protected int AHT2;
        protected int AHT3;
        protected int AHT4;
        protected int AHT5;
        protected int AHT6;
        protected int AHT7;
        protected int AHT8;
        protected int AHT9;
        protected int AHT10;
        protected int AHT11;
        protected int AHT12;
        protected int AHT13;
        protected int AHT14;
        protected int AHT15;
        protected int AHT16;
        protected int AHT17;
        protected int AHT18;
        protected int AHT19;
        protected int AHT20;
        protected int AHT21;
        protected int AHT22;
        protected int AHT23;
        protected int AHT24;
        protected int AHT25;
        protected int AHT26;
        protected int AHT27;
        protected int AHT28;
        protected int AHT29;
        protected int AHT30;
        protected int AHT31;
        protected int AHT32;
        protected int AHT33;
        protected int AHT34;
        protected int AHT35;
        protected int AHT36;
        protected int AHT37;
        protected int AHT38;
        protected int AHT39;
        protected int AHT40;
        protected int AHT41;
        protected int AHT42;
        protected int AHT43;
        protected int AHT44;
        protected int AHT45;
        protected int AHT46;
        protected int AHT47;
        protected int AHT48;
        protected int AHT49;
        protected int AHT50;
        protected int AHT51;
        protected int AHT52;
        protected int AHT53;
        protected int AHT54;
        protected int AHT55;
        protected int AHT56;
        protected int AHT57;
        protected int AHT58;
        protected int AHT59;
        protected int AHT60;
        protected int AHT61;
        protected int AHT62;
        protected int AHT63;
        protected int AHT64;
        protected int AHT65;
        protected int AHT66;
        protected int AHT67;
        protected int AHT68;
        protected int AHT69;
        protected int AHT70;
        protected int AHT71;
        protected int AHT72;
        protected int AHT73;
        protected int AHT74;
        protected int AHT75;
        protected int AHT76;
        protected int AHT77;
        protected int AHT78;
        protected int AHT79;
        protected int AHT80;
        protected int AHT81;
        protected int AHT82;
        protected int AHT83;
        protected int AHT84;
        protected int AHT85;
        protected int AHT86;
        protected int AHT87;
        protected int AHT88;
        protected int AHT89;
        protected int AHT90;
        protected int AHT91;
        protected int AHT92;
        protected int AHT93;
        protected int AHT94;
        protected int AHT95;
        protected int AHT96;
        #endregion
    }
}
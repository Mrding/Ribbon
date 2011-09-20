using System;

namespace Luna.Statistic.Domain
{
    public static class ForecastLibrary
    {
        public const double STAFF_ADJUSTMENT = 0.095;	// when >= 0.005 adjust to 0.1, that is, carry
        public const double SLP_ADJUSTMENT = 0;			// round off(+0.005) seems not a convenstion, changes to "chop"
        public const double MAX_TRAFFIC = 10000;	// max traffic in erlangs
        //short		FORECAST_MODE 		= 1;	// 1: no deducttion calls, 2: deducion only, 3: deduction and adjustoment

        public static double GetServiceLevel(double cv, double aht, double abandonRate, int slSecond, double staff)
        {
            if (staff == 0) return 0d;
            var callsPerHour = Convert.ToInt32((cv - (cv * abandonRate)) * 4);
            var svcLevelPercentage = 0d;
            erlangc(callsPerHour, aht, slSecond, ref svcLevelPercentage, ref staff);

            return svcLevelPercentage + SLP_ADJUSTMENT;
        }



        public static double[] ForecastStaffing(int count, double[] cv, double[] aht, double abandonRate, int slSecond, double[] slGoal, double[][] shrinkage, int startWeekDayIndex, out double[] slGoalExt)
        {
            var staffing = new double[count];
            slGoalExt = new double[count];
            if (count != cv.Length || count != aht.Length || count / 4 != slGoal.Length)
                return staffing;

            for (var i = 0; i < count; i++)
            {
                // Adjust calls to hour and deduct abandoned calls
                var callsPerHour = Convert.ToInt32((cv[i] - (cv[i] * abandonRate)) * 4);

                var quarterIndex = i / 4;

                var goal = slGoal[quarterIndex];
                double slGoalp = goal / 100, tempStaff = 0;
                slGoalExt[i] = slGoalp;
                erlangc(callsPerHour, aht[i], slSecond, ref slGoalp, ref tempStaff);

                var staffingValue = (float)((int)((tempStaff + STAFF_ADJUSTMENT) * 10)) / 10;

                var shrinkagePercentage = shrinkage[startWeekDayIndex][quarterIndex % 24];

                if (shrinkagePercentage == 0)
                    staffing[i] = staffingValue;
                else
                    staffing[i] = staffingValue / shrinkagePercentage;

                if ((i + 1) % 96 == 0)// 整除时代表下一天的开始, 先预先判断下一次是否为明天的开始
                {
                    startWeekDayIndex++;
                    if (startWeekDayIndex > 6)
                        startWeekDayIndex = 0;
                }
            }
            return staffing;
        }


        // Forecast the staff required to achieve the service level goal
        /// <summary>
        /// Compute Staff require to achieve SL goal
        /// </summary>
        /// <param name="count">總共幾筆資料</param>
        /// <param name="callVolume">五分鐘CV</param>
        /// <param name="aht">五分鐘aht</param>
        /// <param name="abandonRate">放棄率</param>
        /// <param name="slSecond">Service Level second</param>
        /// <param name="slPercent">Service Level %</param>
        /// <param name="staff">(輸出值)需求人力</param>
        /// <returns></returns>
        //public static int forecastStaff(int count, int[] callVolume, int[] aht, int abandonRate, int slSecond, int[] slPercent, out double[] staff)
        public static int ForecastStaff(int count, double[] callVolume, double[] aht, double abandonRate, int slSecond, double[] slPercent, out double[] staff)
        {
            staff = new double[count];
            int callsPerHour;
            double slp, tempStaff;
            int tempret, ret = 0;

            //if ((abandonRate < 0) || (abandonRate > 99) || (slSecond < 1) || (count < 1) || (count > 289))
            if ((abandonRate < 0) || (abandonRate > 99) || (slSecond < 1) || (count < 1))
            {
                return -1;
            }

            for (int i = 0; i < count; i++)
            {
                //if (callVolume[i] == 0 || aht[i] == 0)
                //{
                //    staff[i] = 0;
                //    continue;
                //}

                // Adjust calls to hour and deduct abandoned calls
                //callsPerHour = (callVolume[i] - (callVolume[i] * (float)abandonRate/100)) * 4;
                // Version 2: deduct half abandoned calls
                //callsPerHour = Convert.ToInt32((callVolume[i] - (callVolume[i] * (float)abandonRate / 200)) * 4);
                // Version 3: use 5 min time scale
                callsPerHour = Convert.ToInt32((callVolume[i] - (callVolume[i] * (double)abandonRate / 100)) * 12);

                //slp = (double)slPercent[i] / 100;
                slp = slPercent[i] / 100;
                /* a temp version, not working well
                // Service level adjustment for more precise staffing
                // slp = slp / (1-((float)abandonRate/100)*(1-((float)abandonRate/100)));
                */

                //if ((slPercent[i] > 0) && (slPercent[i] < 100) && ((abandonRate + slPercent[i]) < 100))
                if ((slPercent[i] > 0) && (slPercent[i] < 100) && ((slPercent[i]) < 100))
                {
                    tempStaff = 0;
                    tempret = erlangc(callsPerHour, aht[i], slSecond, ref slp, ref tempStaff);
                    staff[i] = (float)((int)((tempStaff + STAFF_ADJUSTMENT) * 10)) / 10;
                    if (tempret == 0) // failure
                        ret = -1;
                }
                else
                {
                    staff[i] = 0;
                    ret = -1;
                }
            }

            return ret;
        }

        public static int ForecastStaffFor15Minutes(int count, double[] callVolume, double[] aht, double abandonRate, int slSecond, double[] slPercent, out double[] staff)
        {
            double[] callVolume5Mins = new double[callVolume.Length];
            for (int i = 0; i < callVolume.Length; i++)
            {
                callVolume5Mins[i] = callVolume[i] / 3;
            }
            return ForecastStaff(count, callVolume5Mins, aht, abandonRate, slSecond, slPercent, out staff);

        }


        // Forecast the answering rate of service level
        /// <summary>
        /// Compute SL% by erlangC of ArrayData
        /// </summary>
        /// <param name="count">總共幾筆資料</param>
        /// <param name="callVolume">五分鐘CV</param>
        /// <param name="aht">五分鐘aht</param>
        /// <param name="abandonRate">放棄率</param>
        /// <param name="slSecond">Service Level second</param>
        /// <param name="slPercent">(輸出值) Service Level %</param>
        /// <param name="staff">排定人力</param>
        /// <returns></returns>
        //public static int forecastServiceLevel(int count, int[] callVolume, int[] aht, int abandonRate, int slSecond, out int[] slPercent, double[] staff)
        public static int forecastServiceLevel(int count, double[] callVolume, double[] aht, int abandonRate, int slSecond, out double[] slPercent, double[] staff)
        {
            //slPercent = new int[count];
            slPercent = new double[count];
            int callsPerHour;
            double slp;
            int tempret, ret = 0;

            //if ((abandonRate < 0) || (abandonRate > 99) || (slSecond < 1) || (count < 1) || (count > 289))
            if ((abandonRate < 0) || (abandonRate > 99) || (slSecond < 1) || (count < 1))
            {
                return -1;
            }

            for (int i = 0; i < count; i++)
            {
                //if (callVolume[i] == 0 || aht[i] == 0)
                //{
                //    slPercent[i] = 0;
                //    continue;
                //}
                // Adjust calls to hour and deduct abandoned calls
                //callsPerHour = (callVolume[i] - (callVolume[i] * (float)abandonRate/100)) * 4;
                // Version 2: deduct half abandoned calls
                //callsPerHour = Convert.ToInt32((callVolume[i] - (callVolume[i] * (float)abandonRate / 200)) * 4);
                // Version 3: use 5 min time scale
                callsPerHour = Convert.ToInt32((callVolume[i] - (callVolume[i] * (float)abandonRate / 200)) * 12);

                // fix by Kenny: No Staffing No SLP
                if (staff[i] == 0)
                {
                    slPercent[i] = 0;
                    continue;
                }

                slp = 0;
                tempret = erlangc(callsPerHour, aht[i], slSecond, ref slp, ref staff[i]);
                /* a temp version, not working well
                // Service level adjustment for more precise service level
                slp = slp * (1-((float)abandonRate/100)*(1-((float)abandonRate/100)));
                */
                //slPercent[i] = (int)((slp + SLP_ADJUSTMENT) * 100);
                slPercent[i] = slp + SLP_ADJUSTMENT;
                if (tempret == -1) // failure
                    ret = -1;
            }

            return ret;
        }

        // Forecast the answering rate of service level 15 mins scale
        /// <summary>
        /// Forecast the answering rate of service level 15 mins scale
        /// </summary>
        /// <param name="count"></param>
        /// <param name="callVolume"></param>
        /// <param name="aht"></param>
        /// <param name="abandonRate"></param>
        /// <param name="slSecond"></param>
        /// <param name="slPercent"></param>
        /// <param name="staff"></param>
        /// <returns></returns>
        public static int forecastServiceLevelFor15Minutes(int count, double[] callVolume, double[] aht, int abandonRate, int slSecond, out double[] slPercent, double[] staff)
        {
            slPercent = new double[count];
            int callsPerHour;
            double slp;
            int tempret, ret = 0;

            if ((abandonRate < 0) || (abandonRate > 99) || (slSecond < 1) || (count < 1))
            {
                return -1;
            }

            for (int i = 0; i < count; i++)
            {
                // use 15 min time scale
                callsPerHour = Convert.ToInt32((callVolume[i] - (callVolume[i] * (float)abandonRate / 200)) * 4);

                // fix by Kenny: No Staffing No SLP
                if (staff[i] == 0)
                {
                    slPercent[i] = 0;
                    continue;
                }
                slp = 0;
                tempret = erlangc(callsPerHour, aht[i], slSecond, ref slp, ref staff[i]);

                if (tempret == -1) // failure
                {
                    ret = -1;
                    //slPercent[i] = 0;
                }
                else
                {
                    slPercent[i] = slp;
                }
            }
            return ret;
        }

        private static int erlangc_old(int callsPerHour, int aht, int slSecond, ref double slPercent, ref double staff)
        {
            // For staff or slPercent, we must give one value to get another value
            // 0 value is for unknown parameter
            if (((slPercent == 0) && (staff == 0)) || ((slPercent != 0) && (staff != 0)))
            {
                return -1;
            }

            float A, N = 1, PreN = 0, P, SLP = 0, PreSLP = 0;
            double T = 1, T1 = 1, T2 = 1;

            A = (float)callsPerHour * aht / 3600; // traffic in erlang

            // Check if traffic too large or too small
            if ((A > MAX_TRAFFIC) || (A < 0))
                return -1;

            if (A == 0)
            {
                return 0;
            }

            // Check if staff is enough
            if ((slPercent == 0) && (staff <= A))
            {
                return -1;
            }

            // N(the server/agent) must > A
            while (N <= A)
            {
                T = T * Convert.ToSingle((A / N)); // An/N!
                T1 = T1 + T; // Sum(Ax/x!)
                PreN = N;
                N = N + 1;
            }

            // Increase N by 1 and check if it achieve the goal
            do
            {
                T2 = T * Convert.ToSingle((A / N) * (N / (N - A)));
                P = (float)(T2 / (T1 + T2));
                PreSLP = SLP;
                SLP = 1 - (P / Convert.ToSingle(Math.Exp(slSecond * (N - A) / aht)));

                // Have we achieved the goal?
                if (((staff == 0) && (SLP >= slPercent)) || ((slPercent == 0) && (N >= staff)))
                    break;

                // Next N
                PreN = N;
                T = T * Convert.ToSingle((A / N));
                T1 = T1 + T;
                N = N + 1;
            }
            while (P > 0.001);

            if (staff == 0)
            {
                if (PreN > A)
                    staff = PreN + (slPercent - PreSLP) / (SLP - PreSLP);
                else
                    staff = A + (N - A) * (slPercent - PreSLP) / (SLP - PreSLP);
            }

            if (slPercent == 0)
            {
                if (PreN > A)
                {
                    if (staff <= N)
                        slPercent = PreSLP + (staff - PreN) * (SLP - PreSLP);
                    else
                        slPercent = 1;
                }
                else
                    slPercent = PreSLP + (staff - A) * (SLP - PreSLP) / (N - A);
            }

            return 0;
        }

        private static int erlangc(int callsPerHour, double aht, int slSecond, ref double slPercent, ref double staff)
        {
            // For staff or slPercent, we must give one value to get another value
            // 0 value is for unknown parameter
            if (((slPercent == 0) && (staff == 0)) || ((slPercent != 0) && (staff != 0)))
            {
                return -1;
            }

            double A, N = 1, PreN = 0, P, SLP = 0, PreSLP = 0;
            double T = 1, T1 = 1, T2 = 1;

            A = callsPerHour * aht / 3600d; // traffic in erlang

            // Check if traffic too large or too small
            //if ((A > MAX_TRAFFIC) || (A < 0))
            //    return -1;

            if (A == 0)
            {
                return 0;
            }

            // Check if staff is enough
            if ((slPercent == 0) && (staff <= A))
            {
                return 0;
                // fix by Kenny: Staff not enought return 0, and SL = 0 dont return exception
                // return -1;
            }

            // N(the server/agent) must > A
            while (N <= A)
            {
                T = T * A / N; // An/N!
                T1 = T1 + T; // Sum(Ax/x!)
                PreN = N;
                N = N + 1;
            }

            // Increase N by 1 and check if it achieve the goal
            do
            {
                T2 = T * A / (N - A);
                P = T2 / (T1 + T2);
                PreSLP = SLP;
                SLP = 1 - (P / Math.Exp(slSecond * (N - A) / (int)aht));

                // Have we achieved the goal?
                if (((staff == 0) && (SLP >= slPercent)) || ((slPercent == 0) && (N >= staff)))
                    break;

                // Next N
                PreN = N;
                T = T * A / N;
                T1 = T1 + T;
                N = N + 1;
            }
            while (P > 0.001);

            if (staff == 0)
            {
                if (PreN > A)
                    staff = PreN + (slPercent - PreSLP) / (SLP - PreSLP);
                else
                    staff = A + (N - A) * (slPercent - PreSLP) / (SLP - PreSLP);
            }

            if (slPercent == 0)
            {
                if (PreN > A)
                {
                    if (staff <= N)
                        slPercent = PreSLP + (staff - PreN) * (SLP - PreSLP);
                    else
                        slPercent = 1;
                }
                else
                    slPercent = PreSLP + (staff - A) * (SLP - PreSLP) / (N - A);
            }

            return 0;
        }


        ///// <summary>
        ///// Forecast the staff required to achieve the service level goal
        ///// </summary>
        ///// <param name="count">Number of 15 minute intervals to forecast. Range: 1-96</param>
        ///// <param name="pCallVolume">Call volume in15 minutes interval</param>
        ///// <param name="pAHT">Average handling time</param>
        ///// <param name="abandonRate">Abandon rate, range: 0-99</param>
        ///// <param name="slSecond">Answering time(in second) of service level goal, range: > 1	</param>
        ///// <param name="pSlPercent">Answering rate(%) of service level goal, range: 1 - 99</param>
        ///// <param name="pStaff">Staff required, the precision is to the first decimal</param>
        ///// <returns>0: success, -1: error</returns>
        //[DllImport("forecast.dll")]
        //public static extern unsafe int forecastStaff(int count, int* pCallVolume, int* pAHT, int abandonRate, int slSecond, int* pSlPercent, float* pStaff);

        ///// <summary>
        ///// Forecast the answering rate of service level from the provided the staff numbers
        ///// </summary>
        ///// <param name="count">Number of 15 minute intervals to forecast. Range: 1-96</param>
        ///// <param name="pCallVolume">Call volume in15 minutes interval</param>
        ///// <param name="pAHT">Average handling time</param>
        ///// <param name="abandonRate">Abandon rate, range: 0-99</param>
        ///// <param name="slSecond">Answering time(in second) of service level goal, range: > 1	</param>
        ///// <param name="pSlPercent">Answering rate(%) of service level goal, range: 1 - 99</param>
        ///// <param name="pStaff">Staff required, the precision is to the first decimal</param>
        ///// <returns>0: success, -1: error</returns>
        //[DllImport("forecast.dll")]
        //public static extern unsafe int forecastServiceLevel(int count, int* pCallVolume, int* pAHT, int abandonRate, int slSecond, int* pSlPercent, float* pStaff);

    }
}

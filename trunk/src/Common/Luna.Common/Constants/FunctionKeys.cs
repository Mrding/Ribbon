namespace Luna.Common.Constants
{
    /// <summary>
    /// 系统所有的 Function Keys
    /// </summary>
    public class FunctionKeys
    {
        #region 預測
        /// <summary>
        /// 預測排班
        /// </summary>
        public const string OpenBP = "OpenBP";
        /// <summary>
        /// 服務水準目標
        /// </summary>
        public const string ServiceLevelGoal = "ServiceLevelGoal";
        /// <summary>
        /// 預測話務量
        /// </summary>
        public const string ForecastManager = "ForecastManager";

        #endregion
        /// <summary>
        /// 人力資源管理
        /// </summary>
        public const string OpenHRWeb = "OpenHRWeb";

        #region 調加減班

        /// <summary>
        /// 發佈加減班公告
        /// </summary>
        public const string AnnounceTuningPost = "AnnounceTuningPost";

        /// <summary>
        /// 查閱加減班公告
        /// </summary>
        public const string ViewTuningPost = "ViewTuningPost";

        /// <summary>
        /// 加減班考核
        /// </summary>
        public const string EstimateTuningResult = "EstimateTuningResult";

        /// <summary>
        /// 批次調度
        /// </summary>
        public const string OpenBatchTuner = "OpenBatchTuner";

        #endregion

        #region 代換班
        /// <summary>
        /// 代換班批閱
        /// </summary>
        public const string DoSwapApproval = "DoSwapApproval";

        /// <summary>
        /// 多日代換班
        /// </summary>
        public const string DoNDaysSwap = "DoNDaysSwap";

        /// <summary>
        /// 代換班群組
        /// </summary>
        public const string ManageSwapGroup = "ManageSwapGroup";

        #endregion

        #region 座席管理

        /// <summary>
        /// 順位規則
        /// </summary>
        public const string ManageSeatSequence = "ManageSeatSequence";

        /// <summary>
        /// 集中規則
        /// </summary>
        public const string ManageSeatConsolidation = "ManageSeatConsolidation";

        #endregion

        /// <summary>
        /// 訓練管理
        /// </summary>
        public const string OpenTrainingWeb = "OpenTrainingWeb";

        #region 現場管理

        /// <summary>
        /// 遵時異常
        /// </summary>
        public const string RTAA = "RTAA";

        /// <summary>
        /// 座席監控
        /// </summary>
        public const string RTSA = "RTSA";

        /// <summary>
        /// 值機狀態類別
        /// </summary>
        public const string ManageAgentStatusType = "ManageAgentStatusType";

        #endregion

        #region 出缺勤管理

        /// <summary>
        /// 異常撫平
        /// </summary>
        public const string RecognizeShrinkage = "RecognizeShrinkage";

        /// <summary>
        /// 異常重新結算
        /// </summary>
        public const string ReIdentifyShrinkage = "ReIdentifyShrinkage";

        /// <summary>
        /// 異常型別設定
        /// </summary>
        public const string ManageShrinkagetype = "ManageShrinkagetype";

        #endregion

        #region 考核

        /// <summary>
        /// 通用事件簿
        /// </summary>
        public const string OpenCommonBook = "OpenCommonBook";

        /// <summary>
        /// 話務事件簿
        /// </summary>
        public const string OpenHistoryBook = "OpenHistoryBook";

        /// <summary>
        /// 日誌簿
        /// </summary>
        public const string OpenDailyBook = "OpenDailyBook";

        /// <summary>
        /// 觀察名單
        /// </summary>
        public const string OpenObserveBook = "OpenObserveBook";

        /// <summary>
        /// 創建通用事件簿
        /// </summary>
        public const string CreateCommonBook = "CreateCommonBook";

        /// <summary>
        /// 申告讚許事件簿
        /// </summary>
        public const string OpenPraiseBook = "OpenPraiseBook";

        /// <summary>
        /// 成績登打作業
        /// </summary>
        public const string AssessmentExtraScore = "AssessmentExtraScore";

        /// <summary>
        /// 群組套用管理
        /// </summary>
        public const string AssessmentItemAdapt = "AssessmentItemAdapt";

        /// <summary>
        /// 群組成績查詢
        /// </summary>
        public const string AssessmentScoreQuery = "AssessmentScoreQuery";

        /// <summary>
        /// 考核事件認列
        /// </summary>
        public const string AssessmentApprove = "AssessmentApprove";

        /// <summary>
        /// 考核條目管理
        /// </summary>
        public const string AssessmentItemManager = "AssessmentItemManager";

        /// <summary>
        /// 配置管理
        /// </summary>
        public const string AssessmentItemPlan = "AssessmentItemPlan";

        /// <summary>
        /// 服務精神考核評分
        /// </summary>
        public const string AssessmentServiceScore = "AssessmentServiceScore";

        /// <summary>
        /// 考核事件類型設定
        /// </summary>
        public const string AssessmentType = "AssessmentType";

        #endregion

        #region 公告

        /// <summary>
        /// 群組公告發佈
        /// </summary>
        public const string PostOrganizationPost = "PostOrganizationPost";

        /// <summary>
        /// 個人公告發佈
        /// </summary>
        public const string PostPrivateMessage = "PostPrivateMessage";

        #endregion

        #region 工具

        /// <summary>
        /// 请假类型管理
        /// </summary>
        public const string ManageAbsentEventType = "ManageAbsentEventType";

        /// <summary>
        /// 工時政策管理
        /// </summary>
        public const string ManageLaborRule = "ManageLaborRule";

        /// <summary>
        /// 組織人員管理
        /// </summary>
        public const string ManageOrganization = "ManageOrganization";

        /// <summary>
        /// 基礎數據匯入
        /// </summary>
        public const string InputBasicData = "InputBasicData";

        /// <summary>
        /// 班表數據匯入
        /// </summary>
        public const string InputSchedule = "InputSchedule";

        /// <summary>
        /// 預測數據匯入
        /// </summary>
        public const string InputForecast = "InputForecast";

        /// <summary>
        /// 公告班表
        /// </summary>
        public const string AnnounceSchedule = "AnnounceSchedule";

        /// <summary>
        /// 通知訊息設定
        /// </summary>
        public const string ManageMessenger = "ManageMessenger";

        /// <summary>
        /// 活动形态設定
        /// </summary>
        public const string ManageAssignmentType = "ManageAssignmentType";

        /// <summary>
        /// 活动形态設定
        /// </summary>
        public const string ManageActivityType = "ManageActivityType";

        /// <summary>
        /// 國定假日設定
        /// </summary>
        public const string ManageNationalHoliday = "ManageNationalHoliday";

        /// <summary>
        /// Acd隊列設定
        /// </summary>
        public const string ManageAcdQueue = "ManageAcdQueue";

        /// <summary>
        /// 技能設定
        /// </summary>
        public const string ManageSkill = "ManageSkill";

        /// <summary>
        /// 服務隊列設定
        /// </summary>
        public const string ManageServiceQueue = "ManageServiceQueue";

        /// <summary>
        /// Acd設定
        /// </summary>
        public const string ManageAcd = "ManageAcd";

        /// <summary>
        /// 登入代號顏色管理
        /// </summary>
        public const string AgentACDIdColor = "AgentACDIdColor";

        /// <summary>
        /// AgentACDID技能對應設定
        /// </summary>
        public const string AgentACDIDMapEmployeeSkill = "AgentACDIDMapEmployeeSkill";

        /// <summary>
        /// 多国日历
        /// </summary>
        public const string CalendarEvent = "CalendarEvent";
        #endregion

        /// <summary>
        /// 報表
        /// </summary>
        public const string OpenReport = "OpenReport";

        #region 任務編組

        /// <summary>
        /// 創建排班期
        /// </summary>
        public const string CreateCS = "CreateCS";

        /// <summary>
        /// 編輯排班期
        /// </summary>
        public const string UpdateCS = "UpdateCS";

        /// <summary>
        /// 參與人員管理
        /// </summary>
        public const string ManageCSAgent = "ManageCSAgent";

        /// <summary>
        ///  班表檢視與調度
        /// </summary>
        public const string TuneSchedule = "TuneSchedule";

        /// <summary>
        ///  啟動排座
        /// </summary>
        public const string StartSeatEngine = "StartSeatEngine";


        /// <summary>
        /// 啟動排班
        /// </summary>
        public const string StartSchedulingEngine = "StartSchedulingEngine";

        /// <summary>
        /// 手動席位調度
        /// </summary>
        public const string ManualArrangeSeat = "ManualArrangeSeat";

        /// <summary>
        /// 排班秘书
        /// </summary>
        public const string ShiftComposer = "ShiftComposer";

        #endregion

        #region 座席管理

        /// <summary>
        /// 新增話房
        /// </summary>
        public const string AddArea = "AddArea";

        /// <summary>
        /// 新增佔席事件
        /// </summary>
        public const string AddSeatEvent = "AddSeatEvent";

        /// <summary>
        /// 佔席事件檢視及修改
        /// </summary>
        public const string UpateSeatEvent = "UpateSeatEvent";

        /// <summary>
        /// 話房規劃
        /// </summary>
        public const string LayOutAreaSeat = "LayOutAreaSeat";

        #endregion
    }
}

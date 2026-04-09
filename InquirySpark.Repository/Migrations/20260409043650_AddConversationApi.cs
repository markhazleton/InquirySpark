using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InquirySpark.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddConversationApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    AuditLogId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActorId = table.Column<int>(type: "INTEGER", nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    EntityId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Changes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.AuditLogId);
                });

            migrationBuilder.CreateTable(
                name: "ChartBuildJob",
                columns: table => new
                {
                    ChartBuildJobId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TriggerType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RequestedById = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestedDt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "getdate()"),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StartedDt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedDt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SuccessCount = table.Column<int>(type: "INTEGER", nullable: false),
                    FailureCount = table.Column<int>(type: "INTEGER", nullable: false),
                    SummaryLog = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartBuildJob", x => x.ChartBuildJobId);
                });

            migrationBuilder.CreateTable(
                name: "ChartDefinition",
                columns: table => new
                {
                    ChartDefinitionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DatasetId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    FilterPayload = table.Column<string>(type: "TEXT", nullable: true),
                    VisualPayload = table.Column<string>(type: "TEXT", nullable: true),
                    CalculationPayload = table.Column<string>(type: "TEXT", nullable: true),
                    VersionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    AutoApprovedFl = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedById = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedDt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "getdate()"),
                    IsArchivedFl = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartDefinition", x => x.ChartDefinitionId);
                });

            migrationBuilder.CreateTable(
                name: "ChartSetting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    SiteAppID = table.Column<int>(type: "INTEGER", nullable: false),
                    SettingType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SettingName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SettingValue = table.Column<string>(type: "TEXT", nullable: false),
                    SettingValueEnhanced = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    CompanyNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CompanyID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyCD = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CompanyDS = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Theme = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    DefaultTheme = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    GalleryFolder = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SiteURL = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Address1 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address2 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FaxNumber = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    DefaultPaymentTerms = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    DefaultInvoiceDescription = table.Column<string>(type: "TEXT", nullable: true),
                    ActiveFL = table.Column<bool>(type: "INTEGER", nullable: false),
                    Component = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    FromEmail = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SMTP = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.CompanyID);
                });

            migrationBuilder.CreateTable(
                name: "DashboardDefinition",
                columns: table => new
                {
                    DashboardDefinitionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DefaultFiltersJson = table.Column<string>(type: "TEXT", nullable: true),
                    LayoutJson = table.Column<string>(type: "TEXT", nullable: true),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PublishedFl = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardDefinition", x => x.DashboardDefinitionId);
                });

            migrationBuilder.CreateTable(
                name: "DeckProject",
                columns: table => new
                {
                    DeckProjectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Theme = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckProject", x => x.DeckProjectId);
                });

            migrationBuilder.CreateTable(
                name: "ImportHistory",
                columns: table => new
                {
                    ImportHistoryID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    ImportType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NumberOfRows = table.Column<int>(type: "INTEGER", nullable: false),
                    ImportLog = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportHistory", x => x.ImportHistoryID);
                });

            migrationBuilder.CreateTable(
                name: "lu_ApplicationType",
                columns: table => new
                {
                    ApplicationTypeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationTypeNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ApplicationTypeDS = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("ApplicationType_PK", x => x.ApplicationTypeID);
                });

            migrationBuilder.CreateTable(
                name: "lu_QuestionType",
                columns: table => new
                {
                    QuestionTypeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionTypeCD = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    QuestionTypeDS = table.Column<string>(type: "TEXT", nullable: false),
                    ControlName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    AnswerDataType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("aaaaaQuestionType_PK", x => x.QuestionTypeID);
                });

            migrationBuilder.CreateTable(
                name: "lu_ReviewStatus",
                columns: table => new
                {
                    ReviewStatusID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReviewStatusNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ReviewStatusDS = table.Column<string>(type: "TEXT", nullable: false),
                    ApprovedFL = table.Column<bool>(type: "INTEGER", nullable: false),
                    CommentFL = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lu_ReviewStatus", x => x.ReviewStatusID);
                });

            migrationBuilder.CreateTable(
                name: "lu_SurveyResponseStatus",
                columns: table => new
                {
                    StatusID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StatusNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StatusDS = table.Column<string>(type: "TEXT", nullable: false),
                    EmailTemplate = table.Column<string>(type: "TEXT", nullable: true),
                    PreviousStatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    NextStatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lu_SurveyResponseStatus", x => x.StatusID);
                });

            migrationBuilder.CreateTable(
                name: "lu_SurveyType",
                columns: table => new
                {
                    SurveyTypeID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyTypeShortNM = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    SurveyTypeNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SurveyTypeDS = table.Column<string>(type: "TEXT", nullable: true),
                    SurveyTypeComment = table.Column<string>(type: "TEXT", nullable: true),
                    ApplicationTypeID = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentSurveyTypeID = table.Column<int>(type: "INTEGER", nullable: true),
                    MutiSequenceFL = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("SurveyType_PK", x => x.SurveyTypeID);
                });

            migrationBuilder.CreateTable(
                name: "lu_UnitOfMeasure",
                columns: table => new
                {
                    UnitOfMeasureID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnitOfMeasureNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UnitOfMeasureDS = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("aaaaaUnitOfMeasure_PK", x => x.UnitOfMeasureID);
                });

            migrationBuilder.CreateTable(
                name: "MetricGroup",
                columns: table => new
                {
                    MetricGroupId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ParentMetricGroupId = table.Column<int>(type: "INTEGER", nullable: true),
                    CalculationType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    QuestionSetRef = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    BenchmarkTarget = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricGroup", x => x.MetricGroupId);
                    table.ForeignKey(
                        name: "FK_MetricGroup_MetricGroup_ParentMetricGroupId",
                        column: x => x.ParentMetricGroupId,
                        principalTable: "MetricGroup",
                        principalColumn: "MetricGroupId");
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleCD = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RoleNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RoleDS = table.Column<string>(type: "TEXT", nullable: false),
                    ReviewLevel = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ReadFL = table.Column<bool>(type: "INTEGER", nullable: false),
                    UpdateFL = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("aaaaaRole_PK", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "SiteRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SurveyResponseAnswer_Error",
                columns: table => new
                {
                    SurveyAnswer_ErrorID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyResponseID = table.Column<int>(type: "INTEGER", nullable: false),
                    SequenceNumber = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    QuestionID = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionAnswerID = table.Column<int>(type: "INTEGER", nullable: true),
                    AnswerType = table.Column<string>(type: "TEXT", nullable: true),
                    AnswerQuantity = table.Column<string>(type: "TEXT", nullable: true),
                    AnswerDate = table.Column<string>(type: "TEXT", nullable: true),
                    AnswerComment = table.Column<string>(type: "TEXT", nullable: true),
                    ErrorCode = table.Column<string>(type: "TEXT", nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    ProgramName = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("aaaaaSurveyResponseAnswer_Error_PK", x => x.SurveyAnswer_ErrorID);
                });

            migrationBuilder.CreateTable(
                name: "tblFiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", unicode: false, maxLength: 50, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", unicode: false, maxLength: 50, nullable: false),
                    Data = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblFiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserPreference",
                columns: table => new
                {
                    UserPreferenceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    PreferenceKey = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PreferenceValue = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedDt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "getdate()"),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreference", x => x.UserPreferenceId);
                });

            migrationBuilder.CreateTable(
                name: "WebPortal",
                columns: table => new
                {
                    WebPortalID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WebPortalNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WebPortalDs = table.Column<string>(type: "TEXT", nullable: true),
                    WebPortalURL = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    WebServiceUrl = table.Column<string>(type: "TEXT", nullable: false),
                    ActiveFl = table.Column<bool>(type: "INTEGER", nullable: true),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebPortal", x => x.WebPortalID);
                });

            migrationBuilder.CreateTable(
                name: "ChartVersion",
                columns: table => new
                {
                    ChartVersionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChartDefinitionId = table.Column<int>(type: "INTEGER", nullable: false),
                    VersionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    SnapshotPayload = table.Column<string>(type: "TEXT", nullable: true),
                    ApprovedFl = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApprovedById = table.Column<int>(type: "INTEGER", nullable: true),
                    ApprovedDt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DiffSummary = table.Column<string>(type: "TEXT", nullable: true),
                    RollbackSourceVersionNumber = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartVersion", x => x.ChartVersionId);
                    table.ForeignKey(
                        name: "FK_ChartVersion_ChartDefinition_ChartDefinitionId",
                        column: x => x.ChartDefinitionId,
                        principalTable: "ChartDefinition",
                        principalColumn: "ChartDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataExportRequest",
                columns: table => new
                {
                    DataExportRequestId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChartDefinitionId = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestedById = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestedDt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FilterPayload = table.Column<string>(type: "TEXT", nullable: true),
                    ColumnSettingsJson = table.Column<string>(type: "TEXT", nullable: true),
                    Format = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RowCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CompletionDt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BlobPath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataExportRequest", x => x.DataExportRequestId);
                    table.ForeignKey(
                        name: "FK_DataExportRequest_ChartDefinition_ChartDefinitionId",
                        column: x => x.ChartDefinitionId,
                        principalTable: "ChartDefinition",
                        principalColumn: "ChartDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GaugeTile",
                columns: table => new
                {
                    GaugeTileId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DashboardDefinitionId = table.Column<int>(type: "INTEGER", nullable: false),
                    MetricNodeId = table.Column<int>(type: "INTEGER", nullable: false),
                    TileType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ThresholdsJson = table.Column<string>(type: "TEXT", nullable: true),
                    DrillTargetUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Size = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ColorPalette = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    TrendSource = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    LastRenderedDt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GaugeTile", x => x.GaugeTileId);
                    table.ForeignKey(
                        name: "FK_GaugeTile_DashboardDefinition_DashboardDefinitionId",
                        column: x => x.DashboardDefinitionId,
                        principalTable: "DashboardDefinition",
                        principalColumn: "DashboardDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    ApplicationID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationNM = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    ApplicationCD = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ApplicationShortNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ApplicationTypeID = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplicationDS = table.Column<string>(type: "TEXT", nullable: true),
                    MenuOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplicationFolder = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false, defaultValueSql: "(N'SurveyAdmin')"),
                    DefaultPageID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((63))"),
                    CompanyID = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("Application_PK", x => x.ApplicationID);
                    table.ForeignKey(
                        name: "FK_Application_Company",
                        column: x => x.CompanyID,
                        principalTable: "Company",
                        principalColumn: "CompanyID");
                    table.ForeignKey(
                        name: "System_FK01",
                        column: x => x.ApplicationTypeID,
                        principalTable: "lu_ApplicationType",
                        principalColumn: "ApplicationTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Survey",
                columns: table => new
                {
                    SurveyID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyTypeID = table.Column<int>(type: "INTEGER", nullable: false),
                    UseQuestionGroupsFL = table.Column<bool>(type: "INTEGER", nullable: false),
                    SurveyNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SurveyShortNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SurveyDS = table.Column<string>(type: "TEXT", nullable: false),
                    CompletionMessage = table.Column<string>(type: "TEXT", nullable: false),
                    ResponseNMTemplate = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ReviewerAccountNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    AutoAssignFilter = table.Column<string>(type: "TEXT", nullable: true),
                    StartDT = table.Column<DateTime>(type: "date", nullable: true),
                    EndDT = table.Column<DateTime>(type: "date", nullable: true),
                    ParentSurveyID = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("Survey_PK", x => x.SurveyID);
                    table.ForeignKey(
                        name: "FK_Survey_SurveyType",
                        column: x => x.SurveyTypeID,
                        principalTable: "lu_SurveyType",
                        principalColumn: "SurveyTypeID");
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    QuestionID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyTypeID = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionShortNM = table.Column<string>(type: "TEXT", maxLength: 75, nullable: false),
                    QuestionNM = table.Column<string>(type: "TEXT", nullable: false),
                    QuestionDS = table.Column<string>(type: "TEXT", nullable: false),
                    Keywords = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    QuestionSort = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewRoleLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionTypeID = table.Column<int>(type: "INTEGER", nullable: false),
                    CommentFL = table.Column<bool>(type: "INTEGER", nullable: false),
                    QuestionValue = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitOfMeasureID = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    FileData = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.QuestionID);
                    table.ForeignKey(
                        name: "FK_Question_QuestionType",
                        column: x => x.QuestionTypeID,
                        principalTable: "lu_QuestionType",
                        principalColumn: "QuestionTypeID");
                    table.ForeignKey(
                        name: "FK_Question_SurveyType",
                        column: x => x.SurveyTypeID,
                        principalTable: "lu_SurveyType",
                        principalColumn: "SurveyTypeID");
                    table.ForeignKey(
                        name: "FK_Question_lu_UnitOfMeasure",
                        column: x => x.UnitOfMeasureID,
                        principalTable: "lu_UnitOfMeasure",
                        principalColumn: "UnitOfMeasureID");
                });

            migrationBuilder.CreateTable(
                name: "MetricScoreSnapshot",
                columns: table => new
                {
                    MetricScoreSnapshotId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MetricGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    SnapshotDt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FilterHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ScoreValue = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    TargetValue = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    SampleSize = table.Column<int>(type: "INTEGER", nullable: false),
                    TrendDelta = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    DataVersionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricScoreSnapshot", x => x.MetricScoreSnapshotId);
                    table.ForeignKey(
                        name: "FK_MetricScoreSnapshot_MetricGroup_MetricGroupId",
                        column: x => x.MetricGroupId,
                        principalTable: "MetricGroup",
                        principalColumn: "MetricGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    ApplicationUserID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstNM = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastNM = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    eMailAddress = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CommentDS = table.Column<string>(type: "TEXT", nullable: true),
                    AccountNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SupervisorAccountNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    LastLoginDT = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastLoginLocation = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false, defaultValueSql: "('Display CompanyName')"),
                    Password = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false, defaultValueSql: "(N'password')"),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RoleID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((4))"),
                    UserKey = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserLogin = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false, defaultValueSql: "('User Login')"),
                    EmailVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    VerifyCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValueSql: "(N'Verify Code')"),
                    CompanyID = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("ApplicationUser_PK", x => x.ApplicationUserID);
                    table.ForeignKey(
                        name: "FK_ApplicationUser_Company",
                        column: x => x.CompanyID,
                        principalTable: "Company",
                        principalColumn: "CompanyID");
                    table.ForeignKey(
                        name: "FK_ApplicationUser_SiteRole",
                        column: x => x.RoleID,
                        principalTable: "SiteRole",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChartAsset",
                columns: table => new
                {
                    ChartAssetId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChartDefinitionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChartVersionId = table.Column<int>(type: "INTEGER", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    GenerationDt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataSnapshotDt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ApprovalStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UsageCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastAccessedDt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CdnBaseUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CommentsJson = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartAsset", x => x.ChartAssetId);
                    table.ForeignKey(
                        name: "FK_ChartAsset_ChartDefinition_ChartDefinitionId",
                        column: x => x.ChartDefinitionId,
                        principalTable: "ChartDefinition",
                        principalColumn: "ChartDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChartAsset_ChartVersion_ChartVersionId",
                        column: x => x.ChartVersionId,
                        principalTable: "ChartVersion",
                        principalColumn: "ChartVersionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChartBuildTask",
                columns: table => new
                {
                    ChartBuildTaskId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChartBuildJobId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChartDefinitionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChartVersionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StartedDt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedDt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ErrorPayload = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartBuildTask", x => x.ChartBuildTaskId);
                    table.ForeignKey(
                        name: "FK_ChartBuildTask_ChartBuildJob_ChartBuildJobId",
                        column: x => x.ChartBuildJobId,
                        principalTable: "ChartBuildJob",
                        principalColumn: "ChartBuildJobId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChartBuildTask_ChartDefinition_ChartDefinitionId",
                        column: x => x.ChartDefinitionId,
                        principalTable: "ChartDefinition",
                        principalColumn: "ChartDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChartBuildTask_ChartVersion_ChartVersionId",
                        column: x => x.ChartVersionId,
                        principalTable: "ChartVersion",
                        principalColumn: "ChartVersionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteAppID = table.Column<int>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppProperty_Application",
                        column: x => x.SiteAppID,
                        principalTable: "Application",
                        principalColumn: "ApplicationID");
                });

            migrationBuilder.CreateTable(
                name: "SiteAppMenu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteAppID = table.Column<int>(type: "INTEGER", nullable: false),
                    MenuText = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TartgetPage = table.Column<string>(type: "TEXT", nullable: false),
                    GlyphName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MenuOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    SiteRoleID = table.Column<int>(type: "INTEGER", nullable: false),
                    ViewInMenu = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteAppMenu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteAppMenu_Application",
                        column: x => x.SiteAppID,
                        principalTable: "Application",
                        principalColumn: "ApplicationID");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationSurvey",
                columns: table => new
                {
                    ApplicationSurveyID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationID = table.Column<int>(type: "INTEGER", nullable: false),
                    SurveyID = table.Column<int>(type: "INTEGER", nullable: false),
                    DefaultRoleID = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("ApplicationSurvey_PK", x => x.ApplicationSurveyID);
                    table.ForeignKey(
                        name: "FK_ApplicationSurvey_Role",
                        column: x => x.DefaultRoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID");
                    table.ForeignKey(
                        name: "SystemSurvey_FK00",
                        column: x => x.SurveyID,
                        principalTable: "Survey",
                        principalColumn: "SurveyID");
                    table.ForeignKey(
                        name: "SystemSurvey_FK01",
                        column: x => x.ApplicationID,
                        principalTable: "Application",
                        principalColumn: "ApplicationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionGroup",
                columns: table => new
                {
                    QuestionGroupID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyID = table.Column<int>(type: "INTEGER", nullable: false),
                    GroupOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionGroupShortNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    QuestionGroupNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    QuestionGroupDS = table.Column<string>(type: "TEXT", nullable: true),
                    QuestionGroupWeight = table.Column<decimal>(type: "decimal(18, 4)", nullable: false, defaultValueSql: "((1))"),
                    GroupHeader = table.Column<string>(type: "TEXT", nullable: true),
                    GroupFooter = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DependentQuestionGroupID = table.Column<int>(type: "INTEGER", nullable: true),
                    DependentMinScore = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    DependentMaxScore = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionGroup", x => x.QuestionGroupID);
                    table.ForeignKey(
                        name: "FK_QuestionGroup_Survey",
                        column: x => x.SurveyID,
                        principalTable: "Survey",
                        principalColumn: "SurveyID");
                });

            migrationBuilder.CreateTable(
                name: "SurveyEmailTemplate",
                columns: table => new
                {
                    SurveyEmailTemplateID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyEmailTemplateNM = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    SurveyID = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    SubjectTemplate = table.Column<string>(type: "TEXT", nullable: false),
                    EmailTemplate = table.Column<string>(type: "TEXT", nullable: false),
                    FromEmailAddress = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    FilterCriteria = table.Column<string>(type: "TEXT", nullable: true),
                    StartDT = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDT = table.Column<DateTime>(type: "datetime", nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    SendToSupervisor = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyEmailTemplate", x => x.SurveyEmailTemplateID);
                    table.ForeignKey(
                        name: "FK_SurveyEmailTemplate_Survey",
                        column: x => x.SurveyID,
                        principalTable: "Survey",
                        principalColumn: "SurveyID");
                });

            migrationBuilder.CreateTable(
                name: "SurveyReviewStatus",
                columns: table => new
                {
                    SurveyReviewStatusID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyID = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewStatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewStatusNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ReviewStatusDS = table.Column<string>(type: "TEXT", nullable: false),
                    ApprovedFL = table.Column<bool>(type: "INTEGER", nullable: false),
                    CommentFL = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyReviewStatus", x => x.SurveyReviewStatusID);
                    table.ForeignKey(
                        name: "FK_SurveyReviewStatus_Survey",
                        column: x => x.SurveyID,
                        principalTable: "Survey",
                        principalColumn: "SurveyID");
                });

            migrationBuilder.CreateTable(
                name: "SurveyStatus",
                columns: table => new
                {
                    SurveyStatusID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyID = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StatusDS = table.Column<string>(type: "TEXT", nullable: false),
                    EmailTemplate = table.Column<string>(type: "TEXT", nullable: false),
                    EmailSubjectTemplate = table.Column<string>(type: "TEXT", nullable: false),
                    PreviousStatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    NextStatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyStatus", x => x.SurveyStatusID);
                    table.ForeignKey(
                        name: "FK_SurveyStatus_Survey",
                        column: x => x.SurveyID,
                        principalTable: "Survey",
                        principalColumn: "SurveyID");
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnswer",
                columns: table => new
                {
                    QuestionAnswerID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionID = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionAnswerSort = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionAnswerShortNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    QuestionAnswerNM = table.Column<string>(type: "TEXT", nullable: false),
                    QuestionAnswerValue = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionAnswerDS = table.Column<string>(type: "TEXT", nullable: false),
                    CommentFL = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActiveFL = table.Column<bool>(type: "INTEGER", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("QuestionAnswer_PK", x => x.QuestionAnswerID);
                    table.ForeignKey(
                        name: "FK_QuestionAnswer_Question",
                        column: x => x.QuestionID,
                        principalTable: "Question",
                        principalColumn: "QuestionID");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserRole",
                columns: table => new
                {
                    ApplicationUserRoleID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationID = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplicationUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleID = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IsDemo = table.Column<bool>(type: "INTEGER", nullable: true),
                    StartUpDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsMonthlyPrice = table.Column<bool>(type: "INTEGER", nullable: true),
                    Price = table.Column<decimal>(type: "money", nullable: true),
                    UserInRolled = table.Column<bool>(type: "INTEGER", nullable: true),
                    IsUserAdmin = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ApplicationUserRole_PK", x => x.ApplicationUserRoleID);
                    table.ForeignKey(
                        name: "FK_ApplicationUserRole_Application",
                        column: x => x.ApplicationID,
                        principalTable: "Application",
                        principalColumn: "ApplicationID");
                    table.ForeignKey(
                        name: "FK_ApplicationUserRole_Role",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID");
                    table.ForeignKey(
                        name: "UserRole_FK01",
                        column: x => x.ApplicationUserID,
                        principalTable: "ApplicationUser",
                        principalColumn: "ApplicationUserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyResponse",
                columns: table => new
                {
                    SurveyResponseID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyResponseNM = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    SurveyID = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplicationID = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedUserID = table.Column<int>(type: "INTEGER", nullable: true),
                    StatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    DataSource = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ConversationId = table.Column<Guid>(type: "TEXT", nullable: false, defaultValueSql: "(lower(hex(randomblob(4))) || '-' || lower(hex(randomblob(2))) || '-4' || substr(lower(hex(randomblob(2))),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(lower(hex(randomblob(2))),2) || '-' || lower(hex(randomblob(6))))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("SurveyResponse_PK", x => x.SurveyResponseID);
                    table.ForeignKey(
                        name: "FK_SurveyResponse_Application",
                        column: x => x.ApplicationID,
                        principalTable: "Application",
                        principalColumn: "ApplicationID");
                    table.ForeignKey(
                        name: "SurveyResponse_FK00",
                        column: x => x.SurveyID,
                        principalTable: "Survey",
                        principalColumn: "SurveyID");
                    table.ForeignKey(
                        name: "SurveyResponse_FK02",
                        column: x => x.AssignedUserID,
                        principalTable: "ApplicationUser",
                        principalColumn: "ApplicationUserID");
                });

            migrationBuilder.CreateTable(
                name: "UserAppProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    AppID = table.Column<int>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAppProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAppProperty_Application",
                        column: x => x.AppID,
                        principalTable: "Application",
                        principalColumn: "ApplicationID");
                    table.ForeignKey(
                        name: "FK_UserAppProperty_ApplicationUser",
                        column: x => x.UserID,
                        principalTable: "ApplicationUser",
                        principalColumn: "ApplicationUserID");
                });

            migrationBuilder.CreateTable(
                name: "UserMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ToUserID = table.Column<int>(type: "INTEGER", nullable: true),
                    FromUserID = table.Column<int>(type: "INTEGER", nullable: true),
                    Message = table.Column<string>(type: "TEXT", nullable: true),
                    Opened = table.Column<bool>(type: "INTEGER", nullable: true),
                    CratedDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: true),
                    AppID = table.Column<int>(type: "INTEGER", nullable: true),
                    ShowonPage = table.Column<int>(type: "INTEGER", nullable: true),
                    FromApp = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMessages_FROM_ApplicationUser",
                        column: x => x.FromUserID,
                        principalTable: "ApplicationUser",
                        principalColumn: "ApplicationUserID");
                    table.ForeignKey(
                        name: "FK_UserMessages_TO_ApplicationUser",
                        column: x => x.ToUserID,
                        principalTable: "ApplicationUser",
                        principalColumn: "ApplicationUserID");
                });

            migrationBuilder.CreateTable(
                name: "ChartAssetFile",
                columns: table => new
                {
                    ChartAssetFileId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChartAssetId = table.Column<int>(type: "INTEGER", nullable: false),
                    Format = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ResolutionHint = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    BlobPath = table.Column<string>(type: "TEXT", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    Checksum = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ExpiresDt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartAssetFile", x => x.ChartAssetFileId);
                    table.ForeignKey(
                        name: "FK_ChartAssetFile_ChartAsset_ChartAssetId",
                        column: x => x.ChartAssetId,
                        principalTable: "ChartAsset",
                        principalColumn: "ChartAssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeckSlide",
                columns: table => new
                {
                    DeckSlideId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeckProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChartAssetId = table.Column<int>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    SlideTitle = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    SlideNotes = table.Column<string>(type: "TEXT", nullable: true),
                    ExportOptionsJson = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckSlide", x => x.DeckSlideId);
                    table.ForeignKey(
                        name: "FK_DeckSlide_ChartAsset_ChartAssetId",
                        column: x => x.ChartAssetId,
                        principalTable: "ChartAsset",
                        principalColumn: "ChartAssetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeckSlide_DeckProject_DeckProjectId",
                        column: x => x.DeckProjectId,
                        principalTable: "DeckProject",
                        principalColumn: "DeckProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionGroupMember",
                columns: table => new
                {
                    QuestionGroupMemberID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionGroupID = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionID = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionWeight = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("QuestionGroupMember_PK", x => x.QuestionGroupMemberID);
                    table.ForeignKey(
                        name: "QuestionGroupMember_FK01",
                        column: x => x.QuestionGroupID,
                        principalTable: "QuestionGroup",
                        principalColumn: "QuestionGroupID");
                    table.ForeignKey(
                        name: "QuestionGroupMember_FK02",
                        column: x => x.QuestionID,
                        principalTable: "Question",
                        principalColumn: "QuestionID");
                });

            migrationBuilder.CreateTable(
                name: "SurveyResponseHistory",
                columns: table => new
                {
                    SurveyResponseHistoryID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    SurveyResponseID = table.Column<int>(type: "INTEGER", nullable: false),
                    SurveyResponseNM = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StatusID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    QuestionGroupID = table.Column<int>(type: "INTEGER", nullable: true),
                    UserNM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Answers = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("aaaaaSurveyResponseHistory_PK", x => x.SurveyResponseHistoryID);
                    table.ForeignKey(
                        name: "FK_SurveyResponseHistory_SurveyResponse",
                        column: x => x.SurveyResponseID,
                        principalTable: "SurveyResponse",
                        principalColumn: "SurveyResponseID");
                });

            migrationBuilder.CreateTable(
                name: "SurveyResponseSequence",
                columns: table => new
                {
                    SurveyResponseSequenceID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyResponseID = table.Column<int>(type: "INTEGER", nullable: false),
                    SequenceNumber = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    SequenceText = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("aaaaaSurveyResponseSequence_PK", x => x.SurveyResponseSequenceID);
                    table.UniqueConstraint("AK_SurveyResponseSequence_SurveyResponseID_SequenceNumber", x => new { x.SurveyResponseID, x.SequenceNumber });
                    table.ForeignKey(
                        name: "SurveyResponseSequence_FK00",
                        column: x => x.SurveyResponseID,
                        principalTable: "SurveyResponse",
                        principalColumn: "SurveyResponseID");
                });

            migrationBuilder.CreateTable(
                name: "SurveyResponseState",
                columns: table => new
                {
                    SurveyResponseStateID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyResponseID = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailBody = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyResponseState", x => x.SurveyResponseStateID);
                    table.ForeignKey(
                        name: "FK_SurveyResponseState_ApplicationUser",
                        column: x => x.AssignedUserID,
                        principalTable: "ApplicationUser",
                        principalColumn: "ApplicationUserID");
                    table.ForeignKey(
                        name: "FK_SurveyResponseState_SurveyResponse",
                        column: x => x.SurveyResponseID,
                        principalTable: "SurveyResponse",
                        principalColumn: "SurveyResponseID");
                    table.ForeignKey(
                        name: "FK_SurveyResponseState_lu_SurveyResponseStatus",
                        column: x => x.StatusID,
                        principalTable: "lu_SurveyResponseStatus",
                        principalColumn: "StatusID");
                });

            migrationBuilder.CreateTable(
                name: "SurveyResponseAnswer",
                columns: table => new
                {
                    SurveyAnswerID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyResponseID = table.Column<int>(type: "INTEGER", nullable: false),
                    SequenceNumber = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    QuestionID = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionAnswerID = table.Column<int>(type: "INTEGER", nullable: false),
                    AnswerType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    AnswerQuantity = table.Column<double>(type: "REAL", nullable: true),
                    AnswerDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    AnswerComment = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedComment = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("SurveyResponseAnswer_PK", x => x.SurveyAnswerID);
                    table.ForeignKey(
                        name: "FK_SurveyResponseAnswer_Question",
                        column: x => x.QuestionID,
                        principalTable: "Question",
                        principalColumn: "QuestionID");
                    table.ForeignKey(
                        name: "FK_SurveyResponseAnswer_QuestionAnswer",
                        column: x => x.QuestionAnswerID,
                        principalTable: "QuestionAnswer",
                        principalColumn: "QuestionAnswerID");
                    table.ForeignKey(
                        name: "FK_SurveyResponseAnswer_SurveyResponseSequence",
                        columns: x => new { x.SurveyResponseID, x.SequenceNumber },
                        principalTable: "SurveyResponseSequence",
                        principalColumns: new[] { "SurveyResponseID", "SequenceNumber" });
                });

            migrationBuilder.CreateTable(
                name: "SurveyResponseAnswerReview",
                columns: table => new
                {
                    SurveyResponseAnswerReviewID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SurveyAnswerID = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplicationUserRoleID = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewLevel = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ReviewStatusID = table.Column<int>(type: "INTEGER", nullable: false),
                    ModifiedID = table.Column<int>(type: "INTEGER", nullable: false, defaultValueSql: "((1))"),
                    ModifiedDT = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedComment = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyResponseAnswerReview", x => x.SurveyResponseAnswerReviewID);
                    table.ForeignKey(
                        name: "FK_SurveyResponseAnswerReview_ApplicationUserRole",
                        column: x => x.ApplicationUserRoleID,
                        principalTable: "ApplicationUserRole",
                        principalColumn: "ApplicationUserRoleID");
                    table.ForeignKey(
                        name: "FK_SurveyResponseAnswerReview_SurveyResponseAnswer",
                        column: x => x.SurveyAnswerID,
                        principalTable: "SurveyResponseAnswer",
                        principalColumn: "SurveyAnswerID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application_ApplicationTypeID",
                table: "Application",
                column: "ApplicationTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Application_CompanyID",
                table: "Application",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSurvey_DefaultRoleID",
                table: "ApplicationSurvey",
                column: "DefaultRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSurvey_SurveyID",
                table: "ApplicationSurvey",
                column: "SurveyID");

            migrationBuilder.CreateIndex(
                name: "UK_ApplicationSurvey",
                table: "ApplicationSurvey",
                columns: new[] { "ApplicationID", "SurveyID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_CompanyID",
                table: "ApplicationUser",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_RoleID",
                table: "ApplicationUser",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "UK_ApplicationUser_AccountNM",
                table: "ApplicationUser",
                column: "AccountNM",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserRole_ApplicationUserID",
                table: "ApplicationUserRole",
                column: "ApplicationUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserRole_RoleID",
                table: "ApplicationUserRole",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "UK_ApplicationUserRole",
                table: "ApplicationUserRole",
                columns: new[] { "ApplicationID", "ApplicationUserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProperty_SiteAppID",
                table: "AppProperty",
                column: "SiteAppID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_ActorId",
                table: "AuditLog",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_CreatedDt",
                table: "AuditLog",
                column: "CreatedDt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_EntityType",
                table: "AuditLog",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_ChartAsset_ChartDefinitionId",
                table: "ChartAsset",
                column: "ChartDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartAsset_ChartVersionId",
                table: "ChartAsset",
                column: "ChartVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartAssetFile_ChartAssetId",
                table: "ChartAssetFile",
                column: "ChartAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartBuildTask_ChartBuildJobId",
                table: "ChartBuildTask",
                column: "ChartBuildJobId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartBuildTask_ChartDefinitionId",
                table: "ChartBuildTask",
                column: "ChartDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartBuildTask_ChartVersionId",
                table: "ChartBuildTask",
                column: "ChartVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartVersion_ChartDefinitionId",
                table: "ChartVersion",
                column: "ChartDefinitionId");

            migrationBuilder.CreateIndex(
                name: "UK_CompanyCD",
                table: "Company",
                column: "CompanyCD",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataExportRequest_ChartDefinitionId",
                table: "DataExportRequest",
                column: "ChartDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckSlide_ChartAssetId",
                table: "DeckSlide",
                column: "ChartAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckSlide_DeckProjectId",
                table: "DeckSlide",
                column: "DeckProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GaugeTile_DashboardDefinitionId",
                table: "GaugeTile",
                column: "DashboardDefinitionId");

            migrationBuilder.CreateIndex(
                name: "UK_lu_ApplicationType_ApplicationTypeNM",
                table: "lu_ApplicationType",
                column: "ApplicationTypeNM",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_lu_SurveyType_SurveyTypeNM",
                table: "lu_SurveyType",
                column: "SurveyTypeNM",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_lu_SurveyType_SurveyTypeShortNM",
                table: "lu_SurveyType",
                column: "SurveyTypeShortNM",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetricGroup_ParentMetricGroupId",
                table: "MetricGroup",
                column: "ParentMetricGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MetricScoreSnapshot_MetricGroupId",
                table: "MetricScoreSnapshot",
                column: "MetricGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionTypeID",
                table: "Question",
                column: "QuestionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Question_SurveyTypeID",
                table: "Question",
                column: "SurveyTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Question_UnitOfMeasureID",
                table: "Question",
                column: "UnitOfMeasureID");

            migrationBuilder.CreateIndex(
                name: "UK_QuestionShortName",
                table: "Question",
                column: "QuestionShortNM",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_QuestionAnswer_ShortNMQuestionID",
                table: "QuestionAnswer",
                columns: new[] { "QuestionID", "QuestionAnswerShortNM" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionGroup_SurveyID",
                table: "QuestionGroup",
                column: "SurveyID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionGroupMember_QuestionGroupID",
                table: "QuestionGroupMember",
                column: "QuestionGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionGroupMember_QuestionID",
                table: "QuestionGroupMember",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_SiteAppMenu_SiteAppID",
                table: "SiteAppMenu",
                column: "SiteAppID");

            migrationBuilder.CreateIndex(
                name: "UK_SiteAppMenu",
                table: "SiteAppMenu",
                columns: new[] { "MenuText", "SiteAppID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Survey_SurveyTypeID",
                table: "Survey",
                column: "SurveyTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyEmailTemplate_SurveyID",
                table: "SurveyEmailTemplate",
                column: "SurveyID");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponse_ApplicationID",
                table: "SurveyResponse",
                column: "ApplicationID");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponse_AssignedUserID",
                table: "SurveyResponse",
                column: "AssignedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponse_ConversationId",
                table: "SurveyResponse",
                column: "ConversationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponse_SurveyID",
                table: "SurveyResponse",
                column: "SurveyID");

            migrationBuilder.CreateIndex(
                name: "SurveyResponse_UK",
                table: "SurveyResponse",
                columns: new[] { "SurveyResponseNM", "AssignedUserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponseAnswer_QuestionAnswerID",
                table: "SurveyResponseAnswer",
                column: "QuestionAnswerID");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponseAnswer_QuestionID",
                table: "SurveyResponseAnswer",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponseAnswer_SurveyResponseID_SequenceNumber",
                table: "SurveyResponseAnswer",
                columns: new[] { "SurveyResponseID", "SequenceNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponseAnswerReview_ApplicationUserRoleID",
                table: "SurveyResponseAnswerReview",
                column: "ApplicationUserRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponseAnswerReview_SurveyAnswerID",
                table: "SurveyResponseAnswerReview",
                column: "SurveyAnswerID");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponseHistory_SurveyResponseID",
                table: "SurveyResponseHistory",
                column: "SurveyResponseID");

            migrationBuilder.CreateIndex(
                name: "UK_SurveyResponseSequence",
                table: "SurveyResponseSequence",
                columns: new[] { "SurveyResponseID", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponseState_AssignedUserID",
                table: "SurveyResponseState",
                column: "AssignedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponseState_StatusID",
                table: "SurveyResponseState",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponseState_SurveyResponseID",
                table: "SurveyResponseState",
                column: "SurveyResponseID");

            migrationBuilder.CreateIndex(
                name: "UK_SurveyReviewStatus_SurveyStatus",
                table: "SurveyReviewStatus",
                columns: new[] { "SurveyID", "ReviewStatusID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_SurveyStatus_SurveyStatus",
                table: "SurveyStatus",
                columns: new[] { "SurveyID", "StatusID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_tblFiles_Type_Name",
                table: "tblFiles",
                columns: new[] { "Name", "ContentType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAppProperty_UserID",
                table: "UserAppProperty",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "UK_UserAppProperty",
                table: "UserAppProperty",
                columns: new[] { "AppID", "UserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_FromUserID",
                table: "UserMessages",
                column: "FromUserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_ToUserID",
                table: "UserMessages",
                column: "ToUserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreference_UserId_PreferenceKey",
                table: "UserPreference",
                columns: new[] { "UserId", "PreferenceKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationSurvey");

            migrationBuilder.DropTable(
                name: "AppProperty");

            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "ChartAssetFile");

            migrationBuilder.DropTable(
                name: "ChartBuildTask");

            migrationBuilder.DropTable(
                name: "ChartSetting");

            migrationBuilder.DropTable(
                name: "DataExportRequest");

            migrationBuilder.DropTable(
                name: "DeckSlide");

            migrationBuilder.DropTable(
                name: "GaugeTile");

            migrationBuilder.DropTable(
                name: "ImportHistory");

            migrationBuilder.DropTable(
                name: "lu_ReviewStatus");

            migrationBuilder.DropTable(
                name: "MetricScoreSnapshot");

            migrationBuilder.DropTable(
                name: "QuestionGroupMember");

            migrationBuilder.DropTable(
                name: "SiteAppMenu");

            migrationBuilder.DropTable(
                name: "SurveyEmailTemplate");

            migrationBuilder.DropTable(
                name: "SurveyResponseAnswer_Error");

            migrationBuilder.DropTable(
                name: "SurveyResponseAnswerReview");

            migrationBuilder.DropTable(
                name: "SurveyResponseHistory");

            migrationBuilder.DropTable(
                name: "SurveyResponseState");

            migrationBuilder.DropTable(
                name: "SurveyReviewStatus");

            migrationBuilder.DropTable(
                name: "SurveyStatus");

            migrationBuilder.DropTable(
                name: "tblFiles");

            migrationBuilder.DropTable(
                name: "UserAppProperty");

            migrationBuilder.DropTable(
                name: "UserMessages");

            migrationBuilder.DropTable(
                name: "UserPreference");

            migrationBuilder.DropTable(
                name: "WebPortal");

            migrationBuilder.DropTable(
                name: "ChartBuildJob");

            migrationBuilder.DropTable(
                name: "ChartAsset");

            migrationBuilder.DropTable(
                name: "DeckProject");

            migrationBuilder.DropTable(
                name: "DashboardDefinition");

            migrationBuilder.DropTable(
                name: "MetricGroup");

            migrationBuilder.DropTable(
                name: "QuestionGroup");

            migrationBuilder.DropTable(
                name: "ApplicationUserRole");

            migrationBuilder.DropTable(
                name: "SurveyResponseAnswer");

            migrationBuilder.DropTable(
                name: "lu_SurveyResponseStatus");

            migrationBuilder.DropTable(
                name: "ChartVersion");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "QuestionAnswer");

            migrationBuilder.DropTable(
                name: "SurveyResponseSequence");

            migrationBuilder.DropTable(
                name: "ChartDefinition");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "SurveyResponse");

            migrationBuilder.DropTable(
                name: "lu_QuestionType");

            migrationBuilder.DropTable(
                name: "lu_UnitOfMeasure");

            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "Survey");

            migrationBuilder.DropTable(
                name: "ApplicationUser");

            migrationBuilder.DropTable(
                name: "lu_ApplicationType");

            migrationBuilder.DropTable(
                name: "lu_SurveyType");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "SiteRole");
        }
    }
}

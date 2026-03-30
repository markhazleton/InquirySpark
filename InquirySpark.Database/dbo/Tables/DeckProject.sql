-- Deck and Dashboard Tables
CREATE TABLE dbo.DeckProject (
    DeckProjectId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    OwnerId INT NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    Theme NVARCHAR(255) NULL,
    CreatedDt DATETIME2 NOT NULL,
    ModifiedDt DATETIME2 NOT NULL,
    CONSTRAINT PK_DeckProject PRIMARY KEY (DeckProjectId)
);

CREATE TABLE dbo.DeckSlide (
    DeckSlideId INT IDENTITY(1,1) NOT NULL,
    DeckProjectId INT NOT NULL,
    ChartAssetId INT NOT NULL,
    SortOrder INT NOT NULL,
    SlideTitle NVARCHAR(255) NULL,
    SlideNotes NVARCHAR(MAX) NULL,
    ExportOptionsJson NVARCHAR(MAX) NULL,
    CONSTRAINT PK_DeckSlide PRIMARY KEY (DeckSlideId),
    CONSTRAINT FK_DeckSlide_DeckProject FOREIGN KEY (DeckProjectId) REFERENCES dbo.DeckProject(DeckProjectId),
    CONSTRAINT FK_DeckSlide_ChartAsset FOREIGN KEY (ChartAssetId) REFERENCES dbo.ChartAsset(ChartAssetId)
);

CREATE TABLE dbo.DashboardDefinition (
    DashboardDefinitionId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Slug NVARCHAR(255) NOT NULL,
    DefaultFiltersJson NVARCHAR(MAX) NULL,
    LayoutJson NVARCHAR(MAX) NULL,
    OwnerId INT NOT NULL,
    CreatedDt DATETIME2 NOT NULL,
    ModifiedDt DATETIME2 NOT NULL,
    PublishedFl BIT NOT NULL,
    CONSTRAINT PK_DashboardDefinition PRIMARY KEY (DashboardDefinitionId)
);

CREATE TABLE dbo.GaugeTile (
    GaugeTileId INT IDENTITY(1,1) NOT NULL,
    DashboardDefinitionId INT NOT NULL,
    MetricNodeId INT NOT NULL,
    TileType NVARCHAR(50) NOT NULL,
    ThresholdsJson NVARCHAR(MAX) NULL,
    DrillTargetUrl NVARCHAR(MAX) NULL,
    Size NVARCHAR(50) NOT NULL,
    ColorPalette NVARCHAR(255) NULL,
    TrendSource NVARCHAR(255) NULL,
    LastRenderedDt DATETIME2 NULL,
    CONSTRAINT PK_GaugeTile PRIMARY KEY (GaugeTileId),
    CONSTRAINT FK_GaugeTile_DashboardDefinition FOREIGN KEY (DashboardDefinitionId) REFERENCES dbo.DashboardDefinition(DashboardDefinitionId)
);

CREATE TABLE dbo.MetricGroup (
    MetricGroupId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ParentMetricGroupId INT NULL,
    CalculationType NVARCHAR(50) NOT NULL,
    Weight DECIMAL(18, 4) NULL,
    QuestionSetRef NVARCHAR(255) NULL,
    BenchmarkTarget DECIMAL(18, 4) NULL,
    DisplayOrder INT NOT NULL,
    CONSTRAINT PK_MetricGroup PRIMARY KEY (MetricGroupId),
    CONSTRAINT FK_MetricGroup_MetricGroup FOREIGN KEY (ParentMetricGroupId) REFERENCES dbo.MetricGroup(MetricGroupId)
);

CREATE TABLE dbo.MetricScoreSnapshot (
    MetricScoreSnapshotId INT IDENTITY(1,1) NOT NULL,
    MetricGroupId INT NOT NULL,
    SnapshotDt DATETIME2 NOT NULL,
    FilterHash NVARCHAR(255) NOT NULL,
    ScoreValue DECIMAL(18, 4) NOT NULL,
    TargetValue DECIMAL(18, 4) NULL,
    SampleSize INT NOT NULL,
    TrendDelta DECIMAL(18, 4) NULL,
    DataVersionId INT NOT NULL,
    CONSTRAINT PK_MetricScoreSnapshot PRIMARY KEY (MetricScoreSnapshotId),
    CONSTRAINT FK_MetricScoreSnapshot_MetricGroup FOREIGN KEY (MetricGroupId) REFERENCES dbo.MetricGroup(MetricGroupId)
);

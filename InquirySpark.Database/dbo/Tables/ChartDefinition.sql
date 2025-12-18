-- Charting Tables
CREATE TABLE dbo.ChartDefinition (
    ChartDefinitionId INT IDENTITY(1,1) NOT NULL,
    DatasetId INT NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Tags NVARCHAR(MAX) NULL,
    FilterPayload NVARCHAR(MAX) NULL,
    VisualPayload NVARCHAR(MAX) NULL,
    CalculationPayload NVARCHAR(MAX) NULL,
    VersionNumber INT NOT NULL,
    AutoApprovedFl BIT NOT NULL,
    CreatedById INT NOT NULL,
    CreatedDt DATETIME2 NOT NULL,
    ModifiedById INT NOT NULL,
    ModifiedDt DATETIME2 NOT NULL,
    IsArchivedFl BIT NOT NULL,
    CONSTRAINT PK_ChartDefinition PRIMARY KEY (ChartDefinitionId)
);

CREATE TABLE dbo.ChartVersion (
    ChartVersionId INT IDENTITY(1,1) NOT NULL,
    ChartDefinitionId INT NOT NULL,
    VersionNumber INT NOT NULL,
    SnapshotPayload NVARCHAR(MAX) NULL,
    ApprovedFl BIT NOT NULL,
    ApprovedById INT NULL,
    ApprovedDt DATETIME2 NULL,
    DiffSummary NVARCHAR(MAX) NULL,
    RollbackSourceVersionNumber INT NULL,
    CONSTRAINT PK_ChartVersion PRIMARY KEY (ChartVersionId),
    CONSTRAINT FK_ChartVersion_ChartDefinition FOREIGN KEY (ChartDefinitionId) REFERENCES dbo.ChartDefinition(ChartDefinitionId)
);

CREATE TABLE dbo.ChartBuildJob (
    ChartBuildJobId INT IDENTITY(1,1) NOT NULL,
    TriggerType NVARCHAR(50) NOT NULL,
    RequestedById INT NOT NULL,
    RequestedDt DATETIME2 NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    StartedDt DATETIME2 NULL,
    CompletedDt DATETIME2 NULL,
    SuccessCount INT NOT NULL,
    FailureCount INT NOT NULL,
    SummaryLog NVARCHAR(MAX) NULL,
    CONSTRAINT PK_ChartBuildJob PRIMARY KEY (ChartBuildJobId)
);

CREATE TABLE dbo.ChartBuildTask (
    ChartBuildTaskId INT IDENTITY(1,1) NOT NULL,
    ChartBuildJobId INT NOT NULL,
    ChartDefinitionId INT NOT NULL,
    ChartVersionId INT NOT NULL,
    Priority INT NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    StartedDt DATETIME2 NULL,
    CompletedDt DATETIME2 NULL,
    ErrorPayload NVARCHAR(MAX) NULL,
    CONSTRAINT PK_ChartBuildTask PRIMARY KEY (ChartBuildTaskId),
    CONSTRAINT FK_ChartBuildTask_ChartBuildJob FOREIGN KEY (ChartBuildJobId) REFERENCES dbo.ChartBuildJob(ChartBuildJobId),
    CONSTRAINT FK_ChartBuildTask_ChartDefinition FOREIGN KEY (ChartDefinitionId) REFERENCES dbo.ChartDefinition(ChartDefinitionId),
    CONSTRAINT FK_ChartBuildTask_ChartVersion FOREIGN KEY (ChartVersionId) REFERENCES dbo.ChartVersion(ChartVersionId)
);

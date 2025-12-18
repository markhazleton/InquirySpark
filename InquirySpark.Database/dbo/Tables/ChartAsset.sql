-- Charting Tables
CREATE TABLE dbo.ChartAsset (
    ChartAssetId INT IDENTITY(1,1) NOT NULL,
    ChartDefinitionId INT NOT NULL,
    ChartVersionId INT NOT NULL,
    DisplayName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Tags NVARCHAR(MAX) NULL,
    GenerationDt DATETIME2 NOT NULL,
    DataSnapshotDt DATETIME2 NOT NULL,
    ApprovalStatus NVARCHAR(50) NOT NULL,
    UsageCount INT NOT NULL,
    LastAccessedDt DATETIME2 NULL,
    CdnBaseUrl NVARCHAR(MAX) NULL,
    CommentsJson NVARCHAR(MAX) NULL,
    CONSTRAINT PK_ChartAsset PRIMARY KEY (ChartAssetId),
    CONSTRAINT FK_ChartAsset_ChartDefinition FOREIGN KEY (ChartDefinitionId) REFERENCES dbo.ChartDefinition(ChartDefinitionId),
    CONSTRAINT FK_ChartAsset_ChartVersion FOREIGN KEY (ChartVersionId) REFERENCES dbo.ChartVersion(ChartVersionId)
);

CREATE TABLE dbo.ChartAssetFile (
    ChartAssetFileId INT IDENTITY(1,1) NOT NULL,
    ChartAssetId INT NOT NULL,
    Format NVARCHAR(50) NOT NULL,
    ResolutionHint NVARCHAR(50) NULL,
    BlobPath NVARCHAR(MAX) NOT NULL,
    FileSizeBytes BIGINT NOT NULL,
    Checksum NVARCHAR(255) NULL,
    ExpiresDt DATETIME2 NULL,
    CONSTRAINT PK_ChartAssetFile PRIMARY KEY (ChartAssetFileId),
    CONSTRAINT FK_ChartAssetFile_ChartAsset FOREIGN KEY (ChartAssetId) REFERENCES dbo.ChartAsset(ChartAssetId)
);

CREATE TABLE dbo.DataExportRequest (
    DataExportRequestId INT IDENTITY(1,1) NOT NULL,
    ChartDefinitionId INT NOT NULL,
    RequestedById INT NOT NULL,
    RequestedDt DATETIME2 NOT NULL,
    FilterPayload NVARCHAR(MAX) NULL,
    ColumnSettingsJson NVARCHAR(MAX) NULL,
    Format NVARCHAR(50) NOT NULL,
    RowCount INT NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CompletionDt DATETIME2 NULL,
    BlobPath NVARCHAR(MAX) NULL,
    CONSTRAINT PK_DataExportRequest PRIMARY KEY (DataExportRequestId),
    CONSTRAINT FK_DataExportRequest_ChartDefinition FOREIGN KEY (ChartDefinitionId) REFERENCES dbo.ChartDefinition(ChartDefinitionId)
);

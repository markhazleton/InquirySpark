CREATE TABLE dbo.AuditLog (
    AuditLogId INT IDENTITY(1,1) NOT NULL,
    ActorId INT NOT NULL,
    EntityType NVARCHAR(255) NOT NULL,
    EntityId NVARCHAR(255) NOT NULL,
    Action NVARCHAR(50) NOT NULL,
    Changes NVARCHAR(MAX) NULL,
    CreatedDt DATETIME2 NOT NULL,
    CONSTRAINT PK_AuditLog PRIMARY KEY (AuditLogId)
);

CREATE INDEX IX_AuditLog_ActorId ON dbo.AuditLog (ActorId);
CREATE INDEX IX_AuditLog_EntityType ON dbo.AuditLog (EntityType);
CREATE INDEX IX_AuditLog_CreatedDt ON dbo.AuditLog (CreatedDt);

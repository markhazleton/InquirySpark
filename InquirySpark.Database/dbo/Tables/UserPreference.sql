CREATE TABLE dbo.UserPreference (
    UserPreferenceId INT IDENTITY(1,1) NOT NULL,
    UserId INT NOT NULL,
    PreferenceKey NVARCHAR(255) NOT NULL,
    PreferenceValue NVARCHAR(MAX) NOT NULL,
    ModifiedDt DATETIME2 NOT NULL,
    RowVersion rowversion NOT NULL,
    CONSTRAINT PK_UserPreference PRIMARY KEY (UserPreferenceId),
    CONSTRAINT UQ_UserPreference_UserId_PreferenceKey UNIQUE (UserId, PreferenceKey)
);

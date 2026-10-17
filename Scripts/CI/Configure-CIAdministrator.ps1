$ErrorActionPreference = 'Stop'

Install-Module SqlServer -Scope CurrentUser -Force

$connectionString = "Server=(localdb)\MSSQLLocalDB;Database=RobsSpacePatrol_CI;Integrated Security=True;TrustServerCertificate=True;"

$sql = @'
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @UserId NVARCHAR(450);
DECLARE @RoleId NVARCHAR(450);

SELECT @UserId = Id
FROM dbo.AspNetUsers
WHERE NormalizedEmail = 'CI-ADMIN@EXAMPLE.COM';

IF @UserId IS NULL
    THROW 50001, 'CI administrator account not found.', 1;

SELECT @RoleId = Id
FROM dbo.AspNetRoles
WHERE NormalizedName = 'ADMIN';

IF @RoleId IS NULL
BEGIN
    SET @RoleId = CONVERT(NVARCHAR(450), NEWID());

    INSERT INTO dbo.AspNetRoles
        (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES
        (@RoleId, 'Admin', 'ADMIN', CONVERT(NVARCHAR(36), NEWID()));
END;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.AspNetUserRoles
    WHERE UserId = @UserId AND RoleId = @RoleId
)
BEGIN
    INSERT INTO dbo.AspNetUserRoles (UserId, RoleId)
    VALUES (@UserId, @RoleId);
END;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.AspNetUserRoles
    WHERE UserId = @UserId AND RoleId = @RoleId
)
    THROW 50002, 'Administrator role assignment failed.', 1;

COMMIT TRANSACTION;

SELECT u.Email, r.Name AS RoleName
FROM dbo.AspNetUserRoles AS ur
INNER JOIN dbo.AspNetUsers AS u ON u.Id = ur.UserId
INNER JOIN dbo.AspNetRoles AS r ON r.Id = ur.RoleId
WHERE u.Id = @UserId AND r.Id = @RoleId;
'@

$result = Invoke-Sqlcmd `
  -ConnectionString $connectionString `
  -Query $sql `
  -ErrorAction Stop

$result | Format-Table -AutoSize

Write-Host "CI administrator configured successfully."

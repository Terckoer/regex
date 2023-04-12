CREATE TABLE tblUsers (
	PK_Users int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	FK_Users_Roles int FOREIGN KEY REFERENCES tblRoles(PK_Roles) NOT NULL,
	Username nvarchar(256) NOT NULL,
	Password_ nvarchar(MAX) NOT NULL,	
	Email nvarchar(256) NOT NULL,
	Email_Confirmed bit NOT NULL,
	Phone_Number nvarchar(MAX),
	Phone_Number_Confirmed bit NOT NULL,
	Enabled_ bit NOT NULL,
	Two_Factor_Enabled bit NOT NULL,
	Security_Stamp nvarchar(MAX),
	Lockout_End_Date_Utc datetime,
	Lockout_Enabled bit NOT NULL,
	Access_Failed_Count int
)


CREATE TABLE tblUsersData (
	PK_UsersData INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	FK_UsersData_Users INT FOREIGN KEY REFERENCES tblUsers(PK_Users) NOT NULL,
	FirstName VARCHAR(255),
	LastName VARCHAR(255),
	CreatedAt DATETIME NOT NULL,
	UpdatedAt DATETIME NOT NULL
)

CREATE TABLE tblRoles (
	PK_Roles INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
	Name_ VARCHAR(50) NOT NULL,
	Description_ VARCHAR(255) NOT NULL
)

-- LAST UPDATE 03/04/2023 01:07PM
CREATE TABLE tblTempTokens(
	PK_Temp_Token int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	FK_TempTokens_Users int NOT NULL FOREIGN KEY REFERENCES tblUsers(PK_Users),
	Token uniqueidentifier NOT NULL DEFAULT NEWID(),
	Creation_Date datetime NOT NULL DEFAULT GETDATE(),
	Expiration_Date datetime NOT NULL DEFAULT DATEADD(MINUTE,15, GETDATE()),
	Enabled_ bit NOT NULL DEFAULT 1
)
Create database DevOpsDB

Use DevOpsDB

DROP TABLE IF EXISTS tblDevOpsUserInfo;
CREATE TABLE tblDevOpsUserInfo (
    User_Unique_id UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    EmailAddress nvarchar(80),
    UserRole integer,
    IsActive bit,
	DepartmentId int,
	FOREIGN KEY (DepartmentId) REFERENCES tblazureDepartment(Id)
);

alter table tblDevOpsUserInfo add Username nvarchar(200);

DROP TABLE IF EXISTS tblTimeSheet;
CREATE TABLE tblTimeSheet (Id int IDENTITY(1,1) PRIMARY KEY,
    UserId uniqueidentifier NOT NULL,
    Project nvarchar(200),
    Epic nvarchar(400),
    Feature nvarchar(400),
	UserStory  nvarchar(400),
	Requirements  nvarchar(400),
	TaskId integer,
    Task  nvarchar(400),
    TimeSheetDate date,
    TimeSheetHours int,
    FOREIGN KEY (UserId) REFERENCES tblDevOpsUserInfo(User_Unique_id)
);

CREATE INDEX index_name ON tblTimeSheet (Id);


Create table tblazureDepartment(Id int IDENTITY(1,1) PRIMARY KEY,Department nvarchar(200));



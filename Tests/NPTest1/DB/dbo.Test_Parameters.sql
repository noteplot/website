USE tst
GO

CREATE TABLE dbo.Test_Parameters(
		ParameterID int identity(1,1) primary key,
		ParameterName nvarchar(128) not null,
		ParameterValue decimal(10,2) null,
		ParameterPrecision	int default 10,
		ParameterScale		int default 0,
		IsNegative			bit default 0
)
GO 

INSERT INTO dbo.Test_Parameters(
		ParameterName,
		ParameterValue,
		ParameterPrecision,
		ParameterScale,
		IsNegative
)
VALUES
('Параметр1',0,10,2,0),
('Параметр2',0,8,3,1),
('Параметр3',0,6,0,0)
go
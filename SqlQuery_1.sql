USE [master]
GO

CREATE DATABASE EmployeeDB
GO

USE [EmployeeDB]
GO

CREATE TABLE Employees
(
	EmployeeID INT IDENTITY(1, 1) CONSTRAINT Pk_Employees_EmployeeID PRIMARY KEY,
	FirstName NVARCHAR(50),
	LastName NVARCHAR(50),
	Email NVARCHAR(100),
	DataOfBirth DATE,
	Salary DECIMAL
)
GO

INSERT INTO Employees(FirstName, LastName, Email, DataOfBirth, Salary)
VALUES
(N'Иванов', N'Иван', 'Ivanov@zxcd.ru', '19990101', 150000),
(N'Петров', N'Петр', 'Petrov@dcxz.ru', '19950101', 120000.25),
(N'Сидоров', N'Сидор(наверно)', 'Sidorov@qwerty.ru', '19950101', 112315.99)
GO

CREATE PROCEDURE spGetAllEmployees
AS 
	SET NOCOUNT ON
	SELECT	EmployeeID,
			FirstName,
			LastName,
			Email,
			DataOfBirth,
			Salary
	FROM Employees
GO

CREATE PROCEDURE spGetEmployeeById
    @EmployeeID INT
AS
BEGIN
    SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM Employees WHERE EmployeeID = @EmployeeID) THEN 1 ELSE 0 END AS BIT) AS Result;
END;
GO


CREATE PROCEDURE spAddEmployee
	@FirstName NVARCHAR(50),
	@LastName NVARCHAR(50),
	@Email NVARCHAR(100),
	@DateOfBirth DATE,
	@Salary DECIMAL
AS
	------
	SET NOCOUNT ON
	IF LEN(@FirstName) = 0 OR LEN(@FirstName) > 50
	BEGIN
		RAISERROR('Имя должно быть от 1 до 50 символов',16, 1);
		RETURN;
	END
	------
	IF LEN(@LastName) = 0 OR LEN(@LastName) > 50
	BEGIN
		RAISERROR('Фамилия должна быть от 1 до 50 символов.', 16, 1);
		RETURN;
	END
	------
	IF LEN(@Email) = 0 OR LEN(@Email) > 100
	BEGIN
		RAISERROR('Email должен быть от 1 до 100 символов.', 16, 1);
		RETURN;
	END
	------
	IF @DateOfBirth IS NULL OR @DateOfBirth >= GETDATE()
	BEGIN
		RAISERROR('Дата рождения должна быть корректной и не в будущем.', 16, 1);
		RETURN;
	END
	------
	IF @Salary < 0
	BEGIN
		RAISERROR('Зарплата не может быть отрицательной.', 16, 1);
		RETURN;
	END
	-- Вставка данных
	BEGIN TRY
		INSERT INTO Employees(FirstName, LastName, Email, DataOfBirth, Salary)
		VALUES(@FirstName, @LastName, @Email, @DateOfBirth, @Salary)
	END TRY
	BEGIN CATCH
		-- Обработка ошибок вставки
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
		  @ErrorMessage = ERROR_MESSAGE(),
		  @ErrorSeverity = ERROR_SEVERITY(),
		  @ErrorState = ERROR_STATE();

		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);		
	END CATCH
GO



CREATE PROCEDURE spUpdateEmployees
	@EmployeeID INT,
	@FieldType INT,
	@TextField NVARCHAR(100) = '',
	@DateOfBirth DATE = NULL,
	@Salary DECIMAL(8,2) = 0
AS
BEGIN
	SET NOCOUNT ON
	-- Ищем запись в БД:
	IF NOT EXISTS (SELECT 1 FROM Employees WHERE EmployeeID = @EmployeeID)
	BEGIN
		RAISERROR('Сотрудника с ID: %d не существует', 16, 1, @EmployeeID);
		RETURN;
	END
	UPDATE Employees
	SET
		FirstName = CASE WHEN @FieldType = 1 THEN @TextField ELSE FirstName END,
		LastName = CASE WHEN @FieldType = 2 THEN @TextField ELSE LastName END,
		Email = CASE WHEN @FieldType = 3 THEN @TextField ELSE Email END,
		DataOfBirth = CASE WHEN @FieldType = 4 THEN @DateOfBirth ELSE DataOfBirth END,
		Salary = CASE WHEN @FieldType = 5 THEN @Salary ELSE Salary END
	WHERE EmployeeID = @EmployeeID;
	-- Если вообще не обновилось ни одно поле:
	IF @@ROWCOUNT = 0
	BEGIN
		RAISERROR('Не было обновлено ни одного поля!', 16, 1);
	END
END;
GO

CREATE PROCEDURE spDeleteEmployee
    @EmployeeID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM Employees 
        WHERE EmployeeID = @EmployeeID;

        SELECT @@ROWCOUNT AS Result;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
/*
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
*/
    END CATCH;
END;

/*
EXEC spGetAllEmployees

EXEC spGetEmployeeById 2

EXEC spUpdateEmployees 2, 3, 'pochta@mail.ru'

EXEC spAddEmployee N'Имя', N'Фамилия', 'imyafamiliya@pochta.ru', '2000-01-01', 3500000.12

EXEC spDeleteEmployee 1007
*/

IF OBJECT_ID(N'Trains.Train') IS NULL
BEGIN
    CREATE TABLE Trains.Train
    (
        TrainID INT NOT NULL IDENTITY(1, 1),
        [Name] NVARCHAR(32) NOT NULL,
        Company NVARCHAR(64) NOT NULL,
        Driver NVARCHAR(32) NOT NULL,
        BaseSpeed INT NOT NULL,
        CarCapacity INT NOT NULL,
        CreatedOn DATETIMEOFFSET NOT NULL DEFAULT(SYSDATETIMEOFFSET()),
        UpdatedOn DATETIMEOFFSET NOT NULL DEFAULT(SYSDATETIMEOFFSET()),
        IsRemoved BINARY(1) NOT NULL DEFAULT(0),

        CONSTRAINT [PK_Trains_Train_TrainID] PRIMARY KEY CLUSTERED
        (
            TrainID ASC
        )
    );
END;

/****************************
 * Unique Constraints
 ****************************/

IF NOT EXISTS
    (
        SELECT *
        FROM sys.key_constraints kc
        WHERE kc.parent_object_id = OBJECT_ID(N'Trains.Train')
            AND kc.[name] = N'UK_Trains_Train_Name'
    )
BEGIN
    ALTER TABLE Trains.Train
    ADD CONSTRAINT [PK_Trains_Train_name] UNIQUE NONCLUSTERED
    (
        [Name] ASC
    )
END;
GO

IF OBJECT_ID(N'Trains.CarType') IS NULL
BEGIN
    CREATE TABLE Trains.CarType
    (
        CarTypeID INT NOT NULL IDENTITY(1, 1),
        [Name] VARCHAR(32) NOT NULL,

        CONSTRAINT PK_Trains_CarType_CarTypeID PRIMARY KEY CLUSTERED
        (
            CarTypeID ASC
        )
    );
END;

/****************************
 * Unique Constraints
 ****************************/

IF NOT EXISTS
    (
        SELECT *
        FROM sys.key_constraints kc
        WHERE kc.parent_object_id = OBJECT_ID(N'Trains.CarType')
            AND kc.[name] = N'UK_Trains_CarType_Name'
    )
BEGIN
    ALTER TABLE Trains.CarType
    ADD CONSTRAINT [UK_Trains_CarType_Name] UNIQUE NONCLUSTERED
    (
        [Name]
    )
END;
GO

IF OBJECT_ID(N'Trains.Passenger') IS NULL
BEGIN
    CREATE TABLE Trains.Passenger
    (
        PassengerID INT NOT NULL IDENTITY(1, 1),
        FirstName NVARCHAR(32) NOT NULL,
        LastName NVARCHAR(32) NOT NULL,
        Email NVARCHAR(128) NOT NULL,
        CreatedOn DATETIMEOFFSET NOT NULL DEFAULT(SYSDATETIMEOFFSET()),
        UpdatedOn DATETIMEOFFSET NOT NULL DEFAULT(SYSDATETIMEOFFSET()),
        IsRemoved BINARY(1) NOT NULL DEFAULT(0),


        CONSTRAINT [PK_Trains.Passenger.PassengerID] PRIMARY KEY CLUSTERED
        (
            PassengerID ASC
        ),
    );
END;

/****************************
 * Unique Constraints
 ****************************/

IF NOT EXISTS
    (
        SELECT *
        FROM sys.key_constraints kc
        WHERE kc.parent_object_id = OBJECT_ID(N'Trains.Passenger')
            AND kc.[name] = N'UK_Trains_Passenger_Email'
    )
BEGIN
    ALTER TABLE Trains.Passenger
    ADD CONSTRAINT [UK_Trains_Passenger_Email] UNIQUE NONCLUSTERED
    (
        Email
    )
END;
GO

IF OBJECT_ID(N'Trains.Car') IS NULL
BEGIN
    CREATE TABLE Trains.Car
    (
        CarID INT NOT NULL IDENTITY(1, 1),
        TrainID INT NOT NULL,
        CarTypeID INT NOT NULL,
        TicketPrice INT NOT NULL,
        PassengerCapacity INT NOT NULL,
        CreatedOn DATETIMEOFFSET NOT NULL DEFAULT(SYSDATETIMEOFFSET()),
        UpdatedOn DATETIMEOFFSET NOT NULL DEFAULT(SYSDATETIMEOFFSET()),
        IsRemoved BINARY(1) NOT NULL DEFAULT(0),

        CONSTRAINT [PK_Trains_Train_CarID] PRIMARY KEY CLUSTERED
        (
            CarID ASC
        ),

        CONSTRAINT FK_Trains_Car_Trains_Train FOREIGN KEY(TrainID)
        REFERENCES Trains.Train(TrainID),

        CONSTRAINT FK_Trains_Car_Trains_CarType FOREIGN KEY(CarTypeID)
        REFERENCES Trains.CarType(CarTypeID)
    );
END;

/****************************
 * Unique Constraints
 ****************************/

 /****************************
 * Foreign Keys Constraints
 ****************************/

IF NOT EXISTS
    (
        SELECT *
        FROM sys.foreign_keys fk
        WHERE fk.parent_object_id = OBJECT_ID(N'Trains.Car')
            AND fk.referenced_object_id = OBJECT_ID(N'Trains.Train')
            AND fk.[name] = N'FK_Trains_Car_Trains_Train'
    )
BEGIN
    ALTER TABLE Train.Car
    ADD CONSTRAINT [FK_Trains_Car_Trains_Train] FOREIGN KEY
    (
        TrainID
    )
    REFERENCES Trains.Train
    (
        TrainID
    );
END;

IF NOT EXISTS
    (
        SELECT *
        FROM sys.foreign_keys fk
        WHERE fk.parent_object_id = OBJECT_ID(N'Trains.Car')
            AND fk.referenced_object_id = OBJECT_ID(N'Trains.CarType')
            AND fk.[name] = N'FK_Trains_Car_Trains_CarType'
    )
BEGIN
    ALTER TABLE Trains.Car
    ADD CONSTRAINT [FK_Trains_Car_Trains_CarType] FOREIGN KEY
    (
        CarTypeID
    )
    REFERENCES Trains.CarType
    (
        CarTypeID
    );
END;
GO

IF OBJECT_ID(N'Trains.Route') IS NULL
BEGIN
    CREATE TABLE Trains.[Route]
    (
        RouteID INT NOT NULL IDENTITY(1, 1),
        TrainID INT NOT NULL,
        DepartureLocation NVARCHAR(64) NOT NULL,
        ArrivalLocation NVARCHAR(64) NOT NULL,
        DepartureTime DATETIMEOFFSET NOT NULL,
        ArrivalTime DATETIMEOFFSET NULL, --Nullable so that we can plug it in later
        Distance INT NOT NULL,
        CreatedOn DATETIMEOFFSET NOT NULL DEFAULT(SYSDATETIMEOFFSET()),
        UpdatedOn DATETIMEOFFSET NOT NULL DEFAULT(SYSDATETIMEOFFSET()),

        CONSTRAINT [PK_Trains.Route.RouteID] PRIMARY KEY CLUSTERED
        (
            RouteID ASC
        ),

        CONSTRAINT FK_Trains_Route_Trains_Train FOREIGN KEY(TrainID)
        REFERENCES Trains.Train(TrainID)
    );
END

/****************************
 * Unique Constraints
 ****************************/

IF NOT EXISTS
    (
        SELECT *
        FROM sys.key_constraints kc
        WHERE kc.parent_object_id = OBJECT_ID(N'Trains.Route')
            AND kc.[name] = N'UK_Trains_Route_DepartureLocation_ArrivalLocation_DepartureTime'
    )
BEGIN
    ALTER TABLE Trains.[Route]
    ADD CONSTRAINT [UK_Trains_Route_DepartureLocation_ArrivalLocation_DepartureTime] UNIQUE NONCLUSTERED
    (
        DepartureLocation,
        ArrivalLocation,
        DepartureTime
    )
END;

/****************************
 * Foreign Keys Constraints
 ****************************/

IF NOT EXISTS
    (
        SELECT *
        FROM sys.foreign_keys fk
        WHERE fk.parent_object_id = OBJECT_ID(N'Trains.Route')
            AND fk.referenced_object_id = OBJECT_ID(N'Trains.Train')
            AND fk.[name] = N'FK_Trains_Route_Trains_Train'
    )
BEGIN
    ALTER TABLE Train.[Route]
    ADD CONSTRAINT [FK_Trains_Route_Trains_Train] FOREIGN KEY
    (
        TrainID
    )
    REFERENCES Trains.Train
    (
        TrainID
    );
END;
GO

IF OBJECT_ID(N'Trains.PassengerRoute') IS NULL
BEGIN
    CREATE TABLE Trains.PassengerRoute
    (
        PassengerRouteID INT NOT NULL IDENTITY(1, 1),
        PassengerID INT NOT NULL,
        RouteID INT NOT NULL,
        CarID INT NOT NULL,
        TicketPrice INT NOT NULL,
        CreatedOn DATETIMEOFFSET NOT NULL DEFAULT(SYSDATETIMEOFFSET()),
        UpdatedOn DATETIMEOFFSET NOT NULL DEFAULT(SYSDATETIMEOFFSET()),

        CONSTRAINT [PK_Trains.PassengerRoute.PassengerRouteID] PRIMARY KEY CLUSTERED
        (
            PassengerRouteID ASC
        ),

        CONSTRAINT FK_Trains_PassengerRoute_Trains_Passenger FOREIGN KEY(PassengerID)
        REFERENCES Trains.Passenger(PassengerID),
        
        CONSTRAINT FK_Trains_PassengerRoute_Trains_Route FOREIGN KEY(RouteID)
        REFERENCES Trains.[Route](RouteID),

        CONSTRAINT FK_Trains_PassengerRoute_Trains_Car FOREIGN KEY(CarID)
        REFERENCES Trains.Car(CarID)
    );
END

/****************************
 * Unique Constraints
 ****************************/

IF NOT EXISTS
    (
        SELECT *
        FROM sys.key_constraints kc
        WHERE kc.parent_object_id = OBJECT_ID(N'Trains.PassengerRoute')
            AND kc.[name] = N'UK_Trains_PassengerRoute_PassengerID_RouteID'
    )
BEGIN
    ALTER TABLE Trains.PassengerRoute
    ADD CONSTRAINT [UK_Trains_PassengerRoute_PassengerID_RouteID] UNIQUE NONCLUSTERED
    (
        PassengerID,
        RouteID
    )
END;

/****************************
 * Foreign Keys Constraints
 ****************************/

IF NOT EXISTS
    (
        SELECT *
        FROM sys.foreign_keys fk
        WHERE fk.parent_object_id = OBJECT_ID(N'Trains.PassengerRoute')
            AND fk.referenced_object_id = OBJECT_ID(N'Trains.Passenger')
            AND fk.[name] = N'FK_Trains_PassengerRoute_Trains_Passenger'
    )
BEGIN
    ALTER TABLE Train.PassengerRoute
    ADD CONSTRAINT [FK_Trains_PassengerRoute_Trains_Passenger] FOREIGN KEY
    (
        PassengerID
    )
    REFERENCES Trains.Passenger
    (
        PassengerID
    );
END;

IF NOT EXISTS
    (
        SELECT *
        FROM sys.foreign_keys fk
        WHERE fk.parent_object_id = OBJECT_ID(N'Trains.PassengerRoute')
            AND fk.referenced_object_id = OBJECT_ID(N'Trains.Route')
            AND fk.[name] = N'FK_Trains_PassengerRoute_Trains_Route'
    )
BEGIN
    ALTER TABLE Train.PassengerRoute
    ADD CONSTRAINT [FK_Trains_PassengerRoute_Trains_Route] FOREIGN KEY
    (
        RouteID
    )
    REFERENCES Trains.[Route]
    (
        RouteID
    );
END;

IF NOT EXISTS
    (
        SELECT *
        FROM sys.foreign_keys fk
        WHERE fk.parent_object_id = OBJECT_ID(N'Trains.PassengerRoute')
            AND fk.referenced_object_id = OBJECT_ID(N'Trains.Car')
            AND fk.[name] = N'FK_Trains_PassengerRoute_Trains_Car'
    )
BEGIN
    ALTER TABLE Train.PassengerRoute
    ADD CONSTRAINT [FK_Trains_PassengerRoute_Trains_Car] FOREIGN KEY
    (
        CarID
    )
    REFERENCES Trains.Car
    (
        CarID
    );
END;
GO

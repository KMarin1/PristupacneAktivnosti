

CREATE TABLE Vrsta (
    Id INT PRIMARY KEY IDENTITY,
    Naziv NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Pristupacnost (
    Id INT PRIMARY KEY IDENTITY,
    Naziv NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Korisnik (
    Id INT PRIMARY KEY IDENTITY,
    KorisnickoIme NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    LozinkaHash NVARCHAR(MAX) NOT NULL,
    Uloga NVARCHAR(20) NOT NULL
);

CREATE TABLE Aktivnost (
    Id INT PRIMARY KEY IDENTITY,
    Naziv NVARCHAR(100) NOT NULL,
    Opis NVARCHAR(1000) NOT NULL,
    Lokacija NVARCHAR(255) NOT NULL,
    Kontakt NVARCHAR(100),
    VrstaId INT NOT NULL,
    FOREIGN KEY (VrstaId) REFERENCES Vrsta(Id)
);

CREATE TABLE AktivnostPristupacnost (
    AktivnostId INT NOT NULL,
    PristupacnostId INT NOT NULL,
    PRIMARY KEY (AktivnostId, PristupacnostId),
    FOREIGN KEY (AktivnostId) REFERENCES Aktivnost(Id),
    FOREIGN KEY (PristupacnostId) REFERENCES Pristupacnost(Id)
);

CREATE TABLE Recenzija (
    Id INT PRIMARY KEY IDENTITY,
    Tekst NVARCHAR(1000),
    Ocjena INT NOT NULL CHECK (Ocjena >= 1 AND Ocjena <= 5),
    AktivnostId INT NOT NULL,
    KorisnikId INT NOT NULL,
    FOREIGN KEY (AktivnostId) REFERENCES Aktivnost(Id),
    FOREIGN KEY (KorisnikId) REFERENCES Korisnik(Id)
);

CREATE TABLE Log (
    Id INT PRIMARY KEY IDENTITY,
    Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
    Level NVARCHAR(20) NOT NULL,
    Poruka NVARCHAR(1000) NOT NULL
);

INSERT INTO Vrsta (Naziv) VALUES 
(N'Smještaj'), (N'Restoran'), (N'Znamenitost');

INSERT INTO Pristupacnost (Naziv) VALUES 
(N'Prijevoz prilagođen invalidskim kolicima'), 
(N'Specijalizirani vodiči'), 
(N'Tumač znakovnog jezika');

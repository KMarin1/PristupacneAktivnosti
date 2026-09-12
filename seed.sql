SET NOCOUNT ON;
BEGIN TRY
    BEGIN TRAN;

    ----------------------------------------------------------------
    -- 1) Vrste
    ----------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM dbo.Vrsta WHERE Naziv = N'Smještaj')
        INSERT INTO dbo.Vrsta (Naziv) VALUES (N'Smještaj');
    IF NOT EXISTS (SELECT 1 FROM dbo.Vrsta WHERE Naziv = N'Restoran')
        INSERT INTO dbo.Vrsta (Naziv) VALUES (N'Restoran');
    IF NOT EXISTS (SELECT 1 FROM dbo.Vrsta WHERE Naziv = N'Znamenitost')
        INSERT INTO dbo.Vrsta (Naziv) VALUES (N'Znamenitost');
    IF NOT EXISTS (SELECT 1 FROM dbo.Vrsta WHERE Naziv = N'Na otvorenom')
        INSERT INTO dbo.Vrsta (Naziv) VALUES (N'Na otvorenom');

    DECLARE @VSmjestaj   int = (SELECT TOP 1 Id FROM dbo.Vrsta WHERE Naziv = N'Smještaj');
    DECLARE @VRestoran   int = (SELECT TOP 1 Id FROM dbo.Vrsta WHERE Naziv = N'Restoran');
    DECLARE @VZnamen     int = (SELECT TOP 1 Id FROM dbo.Vrsta WHERE Naziv = N'Znamenitost');
    DECLARE @VOutdoor    int = (SELECT TOP 1 Id FROM dbo.Vrsta WHERE Naziv = N'Na otvorenom');

    ----------------------------------------------------------------
    -- 2) Pristupačnosti
    ----------------------------------------------------------------
    DECLARE @pr TABLE (Naziv nvarchar(100), Opis nvarchar(500));
    INSERT INTO @pr (Naziv, Opis) VALUES
        (N'Pristup za kolica'),
        (N'WC pristupačan'),
		(N'Rampa pri ulazu'),
		(N'Lift'),
		(N'Braille oznake'),
		(N'Vodiči za slabovidne'),
		(N'Tihi prostor'),
		(N'Prijevozne opcije')

    INSERT INTO dbo.Pristupacnost (Naziv)
    SELECT p.Naziv, p.Opis
    FROM @pr p
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Pristupacnost x WHERE x.Naziv = p.Naziv);

    -- helper IDs
    DECLARE
      @P_Kolica int          = (SELECT TOP 1 Id FROM dbo.Pristupacnost WHERE Naziv=N'Pristup za kolica'),
      @P_WC     int          = (SELECT TOP 1 Id FROM dbo.Pristupacnost WHERE Naziv=N'WC pristupačan'),
      @P_Rampa  int          = (SELECT TOP 1 Id FROM dbo.Pristupacnost WHERE Naziv=N'Rampa pri ulazu'),
      @P_Lift   int          = (SELECT TOP 1 Id FROM dbo.Pristupacnost WHERE Naziv=N'Lift'),
      @P_Braille int         = (SELECT TOP 1 Id FROM dbo.Pristupacnost WHERE Naziv=N'Braille oznake'),
      @P_Vodici int          = (SELECT TOP 1 Id FROM dbo.Pristupacnost WHERE Naziv=N'Vodiči za slabovidne'),
      @P_Tihi   int          = (SELECT TOP 1 Id FROM dbo.Pristupacnost WHERE Naziv=N'Tihi prostor'),
      @P_Prijevoz int        = (SELECT TOP 1 Id FROM dbo.Pristupacnost WHERE Naziv=N'Prijevozne opcije');

    ----------------------------------------------------------------
    -- 3) Aktivnosti
    ----------------------------------------------------------------
    -- A) Hotel Park (Smještaj)
    IF NOT EXISTS (SELECT 1 FROM dbo.Aktivnost WHERE Naziv = N'Hotel Park')
    BEGIN
        INSERT INTO dbo.Aktivnost (Naziv, Opis, Lokacija, Kontakt, VrstaId)
        VALUES (N'Hotel Park', N'Hotel u centru, više pristupačnih soba.', N'Zagreb, Centar', N'recepcija@hotelpark.hr', @VSmjestaj);
    END
    DECLARE @A_HotelPark int = (SELECT TOP 1 Id FROM dbo.Aktivnost WHERE Naziv=N'Hotel Park');

    -- B) Bistro Sunce (Restoran)
    IF NOT EXISTS (SELECT 1 FROM dbo.Aktivnost WHERE Naziv = N'Bistro Sunce')
    BEGIN
        INSERT INTO dbo.Aktivnost (Naziv, Opis, Lokacija, Kontakt, VrstaId)
        VALUES (N'Bistro Sunce', N'Dostupan ulaz i sanitarni čvor.', N'Zagreb, Maksimir', N'kontakt@bistrosunce.hr', @VRestoran);
    END
    DECLARE @A_BistroSunce int = (SELECT TOP 1 Id FROM dbo.Aktivnost WHERE Naziv=N'Bistro Sunce');

    -- C) Muzej Tehnike (Znamenitost)
    IF NOT EXISTS (SELECT 1 FROM dbo.Aktivnost WHERE Naziv = N'Muzej Tehnike')
    BEGIN
        INSERT INTO dbo.Aktivnost (Naziv, Opis, Lokacija, Kontakt, VrstaId)
        VALUES (N'Muzej Tehnike', N'Velike dvorane, vodiči i taktilne ploče.', N'Zagreb, Donji grad', N'info@muzejtehnike.hr', @VZnamen);
    END
    DECLARE @A_Muzej int = (SELECT TOP 1 Id FROM dbo.Aktivnost WHERE Naziv=N'Muzej Tehnike');

    -- D) Botanički vrt (Na otvorenom)
    IF NOT EXISTS (SELECT 1 FROM dbo.Aktivnost WHERE Naziv = N'Botanički vrt')
    BEGIN
        INSERT INTO dbo.Aktivnost (Naziv, Opis, Lokacija, Kontakt, VrstaId)
        VALUES (N'Botanički vrt', N'Staze dostupne kolicima; mirne zone.', N'Zagreb, Donji grad', N'info@botanicki-vrt.hr', @VOutdoor);
    END
    DECLARE @A_Botanicki int = (SELECT TOP 1 Id FROM dbo.Aktivnost WHERE Naziv=N'Botanički vrt');

    -- E) Sljeme – donji dio staze (Na otvorenom)
    IF NOT EXISTS (SELECT 1 FROM dbo.Aktivnost WHERE Naziv = N'Sljeme – donji dio staze')
    BEGIN
        INSERT INTO dbo.Aktivnost (Naziv, Opis, Lokacija, Kontakt, VrstaId)
        VALUES (N'Sljeme – donji dio staze', N'Blaga staza, moguć pristup uz pomoć.', N'Medvednica', N'infocentar@sljeme.hr', @VOutdoor);
    END
    DECLARE @A_Sljeme int = (SELECT TOP 1 Id FROM dbo.Aktivnost WHERE Naziv=N'Sljeme – donji dio staze');

    ----------------------------------------------------------------
    -- 4) Poveži Aktivnosti s Pristupačnostima (AktivnostPristupacnost)
    ----------------------------------------------------------------
    -- helper proc to add link if missing
    IF OBJECT_ID('tempdb..#links') IS NOT NULL DROP TABLE #links;
    CREATE TABLE #links (Aid int, Pid int);
    -- Hotel Park
    INSERT INTO #links VALUES (@A_HotelPark, @P_Kolica), (@A_HotelPark, @P_WC), (@A_HotelPark, @P_Lift), (@A_HotelPark, @P_Prijevoz);
    -- Bistro Sunce
    INSERT INTO #links VALUES (@A_BistroSunce, @P_Rampa), (@A_BistroSunce, @P_WC), (@A_BistroSunce, @P_Kolica);
    -- Muzej Tehnike
    INSERT INTO #links VALUES (@A_Muzej, @P_Lift), (@A_Muzej, @P_Vodici), (@A_Muzej, @P_Braille), (@A_Muzej, @P_Kolica), (@A_Muzej, @P_WC);
    -- Botanički vrt
    INSERT INTO #links VALUES (@A_Botanicki, @P_Kolica), (@A_Botanicki, @P_Tihi);
    -- Sljeme
    INSERT INTO #links VALUES (@A_Sljeme, @P_Prijevoz), (@A_Sljeme, @P_Kolica);

    INSERT INTO dbo.AktivnostPristupacnost (AktivnostId, PristupacnostId)
    SELECT l.Aid, l.Pid
    FROM #links l
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.AktivnostPristupacnost ap
        WHERE ap.AktivnostId = l.Aid AND ap.PristupacnostId = l.Pid
    );

    DROP TABLE #links;

    ----------------------------------------------------------------
    -- 5) Recenzije (par komada, admin i ana)
    ----------------------------------------------------------------
    -- Hotel Park
    IF NOT EXISTS (SELECT 1 FROM dbo.Recenzija WHERE AktivnostId=@A_HotelPark AND KorisnikId=1)
        INSERT INTO dbo.Recenzija (Ocjena, Komentar, CreatedAtUtc, AktivnostId, PristupacnostId, KorisnikId)
        VALUES (5, N'Izvrsna pristupačnost i ljubazno osoblje.', SYSUTCDATETIME(), @A_HotelPark, @P_Lift, 1);

    IF NOT EXISTS (SELECT 1 FROM dbo.Recenzija WHERE AktivnostId=@A_HotelPark AND KorisnikId=1)
        INSERT INTO dbo.Recenzija (Ocjena, Komentar, CreatedAtUtc, AktivnostId, PristupacnostId, KorisnikId)
        VALUES (4, N'Ulaz je jednostavan, WC uredan.', SYSUTCDATETIME(), @A_HotelPark, @P_WC, 1);

    -- Bistro Sunce
    IF NOT EXISTS (SELECT 1 FROM dbo.Recenzija WHERE AktivnostId=@A_BistroSunce AND KorisnikId=1)
        INSERT INTO dbo.Recenzija (Ocjena, Komentar, CreatedAtUtc, AktivnostId, PristupacnostId, KorisnikId)
        VALUES (4, N'Dobra rampa, stolovi razmaknuti.', SYSUTCDATETIME(), @A_BistroSunce, @P_Rampa, 1);

    -- Muzej Tehnike
    IF NOT EXISTS (SELECT 1 FROM dbo.Recenzija WHERE AktivnostId=@A_Muzej AND KorisnikId=1)
        INSERT INTO dbo.Recenzija (Ocjena, Komentar, CreatedAtUtc, AktivnostId, PristupacnostId, KorisnikId)
        VALUES (5, N'Audio vodiči i taktilne ploče su super!', SYSUTCDATETIME(), @A_Muzej, @P_Vodici, 1);

    -- Botanički vrt
    IF NOT EXISTS (SELECT 1 FROM dbo.Recenzija WHERE AktivnostId=@A_Botanicki AND KorisnikId=1)
        INSERT INTO dbo.Recenzija (Ocjena, Komentar, CreatedAtUtc, AktivnostId, PristupacnostId, KorisnikId)
        VALUES (3, N'Lijepo, ali neke staze su uske.', SYSUTCDATETIME(), @A_Botanicki, @P_Kolica, 1);

    -- Sljeme
    IF NOT EXISTS (SELECT 1 FROM dbo.Recenzija WHERE AktivnostId=@A_Sljeme AND KorisnikId=1)
        INSERT INTO dbo.Recenzija (Ocjena, Komentar, CreatedAtUtc, AktivnostId, PristupacnostId, KorisnikId)
        VALUES (3, N'Zahtijeva pomoć, ali izvedivo.', SYSUTCDATETIME(), @A_Sljeme, @P_Prijevoz, 1);

    COMMIT TRAN;
    PRINT 'Seed finished.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;
    THROW;
END CATCH;

-- quick view
SELECT COUNT(*) AS CntVrsta          FROM dbo.Vrsta;
SELECT COUNT(*) AS CntPristupacnost  FROM dbo.Pristupacnost;
SELECT COUNT(*) AS CntAktivnost      FROM dbo.Aktivnost;
SELECT COUNT(*) AS CntAP             FROM dbo.AktivnostPristupacnost;
SELECT COUNT(*) AS CntRecenzija      FROM dbo.Recenzija;


-- SQLite
/*CREATE TABLE drivers2 (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    firstname VARCHAR(40) NOT NULL,
    lastname VARCHAR(40) NOT NULL,
    birthdate DATE DEFAULT '01/01/2000',
    potential INTEGER DEFAULT 65,
	s_turn REAL NOT NULL DEFAULT 50,
	s_break REAL NOT NULL DEFAULT 50,
	s_overtake REAL NOT NULL DEFAULT 50,
	s_defense REAL NOT NULL DEFAULT 50,
	s_tyrecontrol REAL NOT NULL DEFAULT 50,
	s_regularity REAL NOT NULL DEFAULT 50,
	s_reactivity REAL NOT NULL DEFAULT 50
);

CREATE TABLE teams(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name VARCHAR(40),
    idPilot1 INTEGER,
    idPilot2 INTEGER,
    idChassis INTEGER,
    idMotor INTEGER,
    FOREIGN KEY (idPilot1) REFERENCES drivers(id), 
    FOREIGN KEY (idPilot2) REFERENCES drivers(id) ,
    FOREIGN KEY (idChassis) REFERENCES chassis(id) ,
    FOREIGN KEY (idMotor) REFERENCES motors(id) 
    );


CREATE TABLE motors(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name VARCHAR(40),
    power INTEGER,
    fiability INTEGER
);
CREATE TABLE chassis(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name VARCHAR(40)
    aero INTEGER,
    stability INTEGER
);

CREATE TABLE grandprix(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name VARCHAR(40),
    location VARCHAR(40),
    length FLOAT,
    nb_turn INTEGER,
);


*/
/*
INSERT INTO grandprix(name,location,length,nb_turn,max_turn) 
VALUES
--('Grand Prix automobile de Grande-Bretagne','Silverstone','5.891','18','52'),
--('Grand Prix automobile de Italie', 'Monza','5.793','11','53'),
--('Grand Prix automobile de Monaco','Monaco','3.337','18','78');
('Grand Prix automobile de France','Castellet','5.842','15','53'),
('Grand Prix automobile de Allemagne','Hockenheim','4.574','13','67'),
('Grand Prix automobile de Espagne','Madrid','5.474','22','55'),
('Grand Prix automobile de Autriche','Zeltweg','4.318','10','71'),
('Grand Prix automobile de Belgique','Spa Francorchamps','7.004','20','44'),
('Grand Prix automobile de Brésil','Sao Paulo','4.309','15','69'),
('Grand Prix automobile de Etats-Unis','Austin','5.513','20','56'),
('Grand Prix automobile de Canada','Montréal','4.361','13','70'),
('Grand Prix automobile de Chine','Shangai','5.451','16','56'),
('Grand Prix automobile de Qatar','Loasail','5.380','16','57'),
('Grand Prix automobile de Arabie Saoudite','Djeddah','6.175','27','50'),
('Grand Prix automobile de Japon','Suzuka','5.807','17','53'),
('Grand Prix automobile de Australie','Albert Park','5.278','16','58');
*/
--INSERT INTO drivers(firstname,lastname,birthdate,s_turn,s_break,s_overtake,s_defense,s_tyrecontrol)
--VALUES
    --('Lando','Norris','13/11/1999','90','90','88','85','90');
    --('Oscar','Piastri','06/04/2001','90','89','88','87','88');
    --('Charles','Leclerc','16/10/1997','88','90','88','92','90');
    --('Lewis','Hamilton','07/01/1985','87','87','87','84','88');
    --('Max','Verstappen','30/09/1997','90','90','91','91','92');
    --('Isack','Hadjar','28/09/2004','84','83','85','83','83');
    --('George','Russell','15/02/1998','90','87','89','88','93');
    --('Andrea Kimi','Antonelli','25/08/2006','86','84','85','83','80');
    --('Fernando','Alonso','29/07/1981','88','88','90','87','89');
    --('Lance','Stroll','29/10/1998','78','78','80','80','80');
    --('Pierre','Gasly','07/02/1996','87','87','85','88','86');
    --('Franco','Colapinto','27/05/2003','77','77','78','78','77');
    --('Esteban','Ocon','17/09/1996','84','84','84','86','88');
    --('Oliver','Bearman','08/05/2005','82','82','83','83','84');
    --('Liam','Lawson','11/02/2002','81','81','81','82','80');
    --('Arvid','Lindbald','08/08/2007','77','77','78','78','78');
    --('Alexander','Albon','23/03/1996','86','88','87','87','88');
    --('Carlos','Sainz','01/09/1994','88','88','88','85','90');
    --('Gabriel','Bortoletto','14/10/2004','83','83','80','80','83');
    --('Nico','Hulkenberg','19/08/1987','88','88','87','87','90');
    --('Sergio','Perez','26/01/1990','86','86','87','85','85');
    --('Valterri','Bottas','28/08/1989','87','87','83','85','85');


--INSERT INTO motors(name,power,fiability)
--VALUES
    --('Mercesdes Motor','95','90');
    --('Ferrari Motor','88','85');
    --('Ford Motor','85','90');
    --('Honda Motor','85','90');
    --('Audi Motor','85','85');

--INSERT INTO chassis(name,aero,stability)
--VALUES
    --('McLaren Chassis','90','95');
    --('Scuderia Ferrari Chassis','88','93');
    --('Oracle Red Bull Chassis','92','85');
    --('Mercedes-AMG PETRONAS Chassis','92','90');
    --('Aston Martin Chassis','91','86');
    --('BWT Alpine Chassis','85','85');
    --('TGR Haas Chassis','87','87');
    --('Racing Bulls Chassis','88','90');
    --('Atlassian Williams Chassis','90','90');
    --('Revolut Audi Chassis','90','90');
    --('Cadillac Chassis','90','89');

--INSERT INTO teams(name,idPilot1,idPilot2,idChassis,idMotor)
--VALUES
    --('McLaren Mastercard Formula 1 Team','4','5','1','1');
    --('Scuderia Ferrari HP','6','9','2','2');
    --('Oracle Red Bull Racing','10','11','3','3');
    --('Mercedes-AMG PETRONAS Formula One Team','12','13','4','1');
    --('Aston Martin Aramco Formula One Team','14','15','5','4');
    --('BWT Alpine Formula One Team','16','17','6','1');
    --('TGR Haas F1 Team','18','19','7','2');
    --('Visa Cash App Racing Bulls Formula One Team','20','21','8','3');
    --('Atlassian Williams F1 Team','22','23','9','1');
    --('Revolut Audi F1 Team','24','25','10','5');
    --('Cadillac Formula 1 Team','26','27','11','2');


SELECT sqlite_version();
/*ALTER TABLE grandprix
DROP COLUMN division_racing;

ALTER TABLE grandprix
ADD COLUMN f1_racing BOOLEAN NOT NULL DEFAULT TRUE;

ALTER TABLE grandprix
ADD COLUMN f2_racing BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE grandprix
ADD COLUMN f3_racing BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE grandprix
ADD COLUMN fe_racing BOOLEAN NOT NULL DEFAULT FALSE;

ALTER TABLE grandprix
ADD COLUMN facademy_racing BOOLEAN NOT NULL DEFAULT FALSE;*/

--UPDATE  grandprix 
--SET  division_racing = division_racing || 'F2'
--WHERE id = '3';
UPDATE drivers 
SET s_turn = '51', s_break = '50', s_overtake = '50', s_defense = '50', s_tyrecontrol = '50' 
WHERE ID = '49';
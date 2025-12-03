
-- SQLite
/*CREATE TABLE drivers(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    firstname VARCHAR(40),
    lastname VARCHAR(40),
    birthdate DATE
);

CREATE TABLE teams(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name VARCHAR(40)
    FOREIGN KEY (idPilot1) REFERENCES drivers(id) 
    FOREIGN KEY (idPilot2) REFERENCES drivers(id) 
    FOREIGN KEY (Chassis) REFERENCES drivers(id) 
    FOREIGN KEY (Motor) REFERENCES drivers(id) 
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

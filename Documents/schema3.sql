CREATE TABLE `test`.`reaktor` (
	`id_reaktor` INT NOT NULL,
	`czy_jest_wlaczony` BIT NULL,
	PRIMARY KEY (`id_reaktor`)
);


CREATE TABLE `test`.`blok_reaktora` (
	`id_blok_reaktora` INT NOT NULL,
	`czy_pret_paliwowy_jest_do_wymiany` BIT NULL,
	`numer_preta_paliwowego` INT NULL,
	`id_reaktor` INT NULL,
	PRIMARY KEY (`id_blok_reaktora`)
);


CREATE TABLE `test`.`czujniki` (
	`cisnienie` decimal(10,0) DEFAULT NULL,
	`produkowana_energia` decimal(10,0) DEFAULT NULL,
	`stan_paliwa` decimal(10,0) DEFAULT NULL,
	`temperatura` decimal(10,0) DEFAULT NULL,
	`id_czujniki` int(11) NOT NULL,
	`id_blok_reaktora` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `test`.`awaria` (
	`id_awaria` INT NOT NULL,
	`data_naprawienia` DATE NULL,
	`data_wystapienia` DATE NOT NULL,
	`szczegoly` TINYTEXT NULL,
	`id_blok_reaktora` INT NOT NULL,
	`id_reaktor` INT NOT NULL,
	PRIMARY KEY (`id_awaria`),
	FOREIGN KEY (`id_blok_reaktora`) REFERENCES blok_reaktora(`id_blok_reaktora`),
	FOREIGN KEY (`id_reaktor`) REFERENCES reaktor(`id_reaktor`)
);
  
CREATE TABLE `test`.`elektrowniajadrowa`(
	`id_elektrowniajadrowa` INT NOT NULL,
	`ilosc_zatrudnionych_ludzi` INT NOT NULL,
	`lokalizacja` VARCHAR(300),
	PRIMARY KEY (`id_elektrowniajadrowa`)
);
  
CREATE TABLE `test`.`pracownikelektrowni`(
	`id_pracownikelektrowni` INT NOT NULL,
	`id_podleglypracownikelektrowni` INT NULL,
	`adres` VARCHAR (300) NOT NULL,
	`adres_email` VARCHAR (100) NOT NULL,
	`data_urodzenia` DATE NOT NULL,
	`imie` VARCHAR (30) NOT NULL,
	`nazwisko` VARCHAR (50) NOT NULL,
	`numer_pracownika` INT NOT NULL,
	`numer_telefonu` VARCHAR (12) NOT NULL,
	`pesel` VARCHAR (11) NOT NULL,
	`wiek` INT NOT NULL,
	`id_reaktor` INT NOT NULL,
	PRIMARY KEY (`id_pracownikelektrowni`),
	FOREIGN KEY (`id_reaktor`) REFERENCES reaktor(`id_reaktor`),
	FOREIGN KEY (`id_podleglypracownikelektrowni`) REFERENCES pracownikelektrowni(`id_pracownikelektrowni`)
);
  
CREATE TABLE `test`.`zatrudnienie`(
	`id_zatrudnienie` INT NOT NULL,
	`data_wygasniecia_umowy` DATE NULL,
	`data_zawarcia_umowy` DATE NOT NULL,
	`placa` decimal(18,2) NOT NULL,
	`rodzaj_umowy` VARCHAR(50),
	`stanowisko` VARCHAR(50),
	`id_zatrudnienie_elektrownia_jadrowa` INT NOT NULL,
	`id_zatrudnienie_pracownika_elektrowni` INT NOT NULL,
	PRIMARY KEY (`id_zatrudnienie`),
	FOREIGN KEY (`id_zatrudnienie_elektrownia_jadrowa`) REFERENCES elektrowniajadrowa(`id_elektrowniajadrowa`),
	FOREIGN KEY (`id_zatrudnienie_pracownika_elektrowni`) REFERENCES pracownikelektrowni(`id_pracownikelektrowni`)
);
  
CREATE TABLE `test`.`zadanie`(
	`id_zadanie` INT NOT NULL,
	`data_przydzielenia` DATE NOT NULL,
	`data_wykonania` DATE NULL,
	`priorytet` INT NOT NULL,
	`tresc` VARCHAR (300) NOT NULL,
	`id_pracownikelektrowni` INT NOT NULL,
	PRIMARY KEY (`id_zadanie`),
	FOREIGN KEY (`id_pracownikelektrowni`) REFERENCES pracownikelektrowni(`id_pracownikelektrowni`)
);
  
CREATE TABLE `test`.`administrator`(
	`id_administrator` INT NOT NULL,
	PRIMARY KEY (`id_administrator`),
	FOREIGN KEY (`id_administrator`) REFERENCES pracownikelektrowni(`id_pracownikelektrowni`)
);
  
CREATE TABLE `test`.`system`(
	`id_system` INT NOT NULL,
	`id_administrator` INT NOT NULL,
	PRIMARY KEY (`id_system`),
	FOREIGN KEY (`id_administrator`) REFERENCES administrator(`id_administrator`)
);
  
  
CREATE TABLE `test`.`kierownikkadr`(
	`id_kierownikkadr` INT NOT NULL,
	PRIMARY KEY (`id_kierownikkadr`),
	FOREIGN KEY (`id_kierownikkadr`) REFERENCES pracownikelektrowni(`id_pracownikelektrowni`)
);
  
CREATE TABLE `test`.`grafik`(
	`id_grafik` INT NOT NULL,
	`ilosc_harmonogramow` INT NULL,
	`id_kierownikkadr` INT NOT NULL,
	PRIMARY KEY (`id_grafik`),
	FOREIGN KEY (`id_kierownikkadr`) REFERENCES kierownikkadr(`id_kierownikkadr`)
);
  
CREATE TABLE `test`.`zmiana`(
	`id_zmiana` INT NOT NULL,
	`id_pracownikelektrowni` INT NOT NULL,
	`id_grafik` INT NOT NULL,
	`data_godzina_konca` DATETIME NOT NULL,
	`data_godzina_rozpoczecia` DATETIME NOT NULL,
	PRIMARY KEY (`id_zmiana`),
	FOREIGN KEY (`id_grafik`) REFERENCES grafik(`id_grafik`),
	FOREIGN KEY (`id_pracownikelektrowni`) REFERENCES pracownikelektrowni(`id_pracownikelektrowni`)
);
  
CREATE TABLE `test`.`kierowniklogistyki`(
	`id_kierowniklogistyki` INT NOT NULL,
	PRIMARY KEY (`id_kierowniklogistyki`),
	FOREIGN KEY (`id_kierowniklogistyki`) REFERENCES pracownikelektrowni(`id_pracownikelektrowni`)
);
  
CREATE TABLE `test`.`zamowienie`(
	`id_zamowienie` INT NOT NULL,
	`id_kierowniklogistyki` INT NOT NULL,
	`data_wykonania` DATE NULL,
	`data_zalozenia` DATE NOT NULL,
	`dostawca` VARCHAR (50) NOT NULL,
	`producent` VARCHAR (50) NOT NULL,
	`rodzaj_sprzetu` VARCHAR (50) NOT NULL,
	PRIMARY KEY (`id_zamowienie`),
	FOREIGN KEY (`id_kierowniklogistyki`) REFERENCES kierowniklogistyki(`id_kierowniklogistyki`)
);
  
CREATE TABLE `test`.`sprzet`(
	`id_sprzet` INT NOT NULL,
	`id_magazyn` INT NOT NULL,
	`ilosc` INT NOT NULL,
	`typ` VARCHAR(50),
	PRIMARY KEY (`id_sprzet`),
	FOREIGN KEY (`id_kierowniklogistyki`) REFERENCES kierowniklogistyki(`id_kierowniklogistyki`)
);
  
CREATE TABLE `test`.`magazyn`(
	`id_magazyn` INT NOT NULL,
	`id_kierowniklogistyki` INT NOT NULL,
	`pojemnosc` INT NOT NULL,
	`id_sprzet` INT NOT NULL,
	PRIMARY KEY (`id_magazyn`),
	FOREIGN KEY (`id_sprzet`) REFERENCES sprzet(`id_sprzet`)
);
  
CREATE TABLE `test`.`permissions` (
	`idpermissions` INT NOT NULL,
	`user` VARCHAR(45) NOT NULL,
	`administrator` INT NULL,
	`awaria` INT NULL,
	`blok_reaktora` INT NULL,
	`czujniki` INT NULL,
	`grafik` INT NULL,
	`kierownik_kadr` INT NULL,
	`kierownik_logistyki` INT NULL,
	`magazyn` INT NULL,
	`pracownik_elektrowni` INT NULL,
	`reaktor` INT NULL,
	`sprzet` INT NULL,
	`system` INT NULL,
	`zadanie` INT NULL,
	`zamowienie` INT NULL,
	`zatrudnienie` INT NULL,
	`zmiana` INT NULL,
	PRIMARY KEY (`idpermissions`),
	UNIQUE INDEX `user_UNIQUE` (`user` ASC)
);

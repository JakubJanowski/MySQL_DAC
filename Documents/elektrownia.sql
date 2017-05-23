CREATE TABLE `administrator` (
  `id_administrator` int(11) NOT NULL,
  PRIMARY KEY (`id_administrator`),
  CONSTRAINT `administrator_ibfk_1` FOREIGN KEY (`id_administrator`) REFERENCES `pracownikelektrowni` (`id_pracownikelektrowni`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `awaria` (
  `id_awaria` int(11) NOT NULL,
  `data_naprawienia` date DEFAULT NULL,
  `data_wystapienia` date NOT NULL,
  `szczegoly` tinytext,
  `id_blok_reaktora` int(11) NOT NULL,
  `id_reaktor` int(11) NOT NULL,
  PRIMARY KEY (`id_awaria`),
  KEY `id_blok_reaktora` (`id_blok_reaktora`),
  KEY `id_reaktor` (`id_reaktor`),
  CONSTRAINT `awaria_ibfk_1` FOREIGN KEY (`id_blok_reaktora`) REFERENCES `blok_reaktora` (`id_blok_reaktora`),
  CONSTRAINT `awaria_ibfk_2` FOREIGN KEY (`id_reaktor`) REFERENCES `reaktor` (`id_reaktor`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `blok_reaktora` (
  `id_blok_reaktora` int(11) NOT NULL,
  `czy_pret_paliwowy_jest_do_wymiany` bit(1) DEFAULT NULL,
  `numer_preta_paliwowego` int(11) DEFAULT NULL,
  `id_reaktor` int(11) DEFAULT NULL,
  PRIMARY KEY (`id_blok_reaktora`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `grafik` (
  `id_grafik` int(11) NOT NULL,
  `ilosc_harmonogramow` int(11) DEFAULT NULL,
  `id_kierownikkadr` int(11) NOT NULL,
  PRIMARY KEY (`id_grafik`),
  KEY `id_kierownikkadr` (`id_kierownikkadr`),
  CONSTRAINT `grafik_ibfk_1` FOREIGN KEY (`id_kierownikkadr`) REFERENCES `kierownikkadr` (`id_kierownikkadr`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `czujniki` (
  `cisnienie` decimal(10,0) DEFAULT NULL,
  `produkowana_energia` decimal(10,0) DEFAULT NULL,
  `stan_paliwa` decimal(10,0) DEFAULT NULL,
  `temperatura` decimal(10,0) DEFAULT NULL,
  `id_czujniki` int(11) NOT NULL,
  `id_blok_reaktora` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `kierownikkadr` (
  `id_kierownikkadr` int(11) NOT NULL,
  PRIMARY KEY (`id_kierownikkadr`),
  CONSTRAINT `kierownikkadr_ibfk_1` FOREIGN KEY (`id_kierownikkadr`) REFERENCES `pracownikelektrowni` (`id_pracownikelektrowni`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `kierowniklogistyki` (
  `id_kierowniklogistyki` int(11) NOT NULL,
  PRIMARY KEY (`id_kierowniklogistyki`),
  CONSTRAINT `kierowniklogistyki_ibfk_1` FOREIGN KEY (`id_kierowniklogistyki`) REFERENCES `pracownikelektrowni` (`id_pracownikelektrowni`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `permissions` (
  `idpermissions` int(11) NOT NULL AUTO_INCREMENT,
  `user` varchar(45) NOT NULL,
  `administrator` int(11) DEFAULT NULL,
  `awaria` int(11) DEFAULT NULL,
  `blok_reaktora` int(11) DEFAULT NULL,
  `czujniki` int(11) DEFAULT NULL,
  `grafik` int(11) DEFAULT NULL,
  `kierownikkadr` int(11) DEFAULT NULL,
  `kierowniklogistyki` int(11) DEFAULT NULL,
  `magazyn` int(11) DEFAULT NULL,
  `pracownikelektrowni` int(11) DEFAULT NULL,
  `reaktor` int(11) DEFAULT NULL,
  `sprzet` int(11) DEFAULT NULL,
  `system` int(11) DEFAULT NULL,
  `zadanie` int(11) DEFAULT NULL,
  `zamowienie` int(11) DEFAULT NULL,
  `zatrudnienie` int(11) DEFAULT NULL,
  `zmiana` int(11) DEFAULT NULL,
  `userPermissions` int(11) DEFAULT '32768',
  `giverId` int(11) DEFAULT NULL,
  PRIMARY KEY (`idpermissions`),
  UNIQUE KEY `user_UNIQUE` (`user`)
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=latin1;

CREATE TABLE `magazyn` (
  `id_magazyn` int(11) NOT NULL,
  `id_kierowniklogistyki` int(11) NOT NULL,
  `pojemnosc` int(11) NOT NULL,
  `id_sprzet` int(11) NOT NULL,
  PRIMARY KEY (`id_magazyn`),
  KEY `id_sprzet` (`id_sprzet`),
  CONSTRAINT `magazyn_ibfk_1` FOREIGN KEY (`id_sprzet`) REFERENCES `sprzet` (`id_sprzet`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `pracownikelektrowni` (
  `id_pracownikelektrowni` int(11) NOT NULL,
  `id_podleglypracownikelektrowni` int(11) DEFAULT NULL,
  `adres` varchar(300) NOT NULL,
  `adres_email` varchar(100) NOT NULL,
  `data_urodzenia` date NOT NULL,
  `imie` varchar(30) NOT NULL,
  `nazwisko` varchar(50) NOT NULL,
  `numer_pracownika` int(11) NOT NULL,
  `numer_telefonu` varchar(12) NOT NULL,
  `pesel` varchar(11) NOT NULL,
  `wiek` int(11) NOT NULL,
  `id_reaktor` int(11) NOT NULL,
  PRIMARY KEY (`id_pracownikelektrowni`),
  KEY `id_reaktor` (`id_reaktor`),
  KEY `id_podleglypracownikelektrowni` (`id_podleglypracownikelektrowni`),
  CONSTRAINT `pracownikelektrowni_ibfk_1` FOREIGN KEY (`id_reaktor`) REFERENCES `reaktor` (`id_reaktor`),
  CONSTRAINT `pracownikelektrowni_ibfk_2` FOREIGN KEY (`id_podleglypracownikelektrowni`) REFERENCES `pracownikelektrowni` (`id_pracownikelektrowni`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `reaktor` (
  `id_reaktor` int(11) NOT NULL,
  `czy_jest_wlaczony` bit(1) DEFAULT NULL,
  PRIMARY KEY (`id_reaktor`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `sprzet` (
  `id_sprzet` int(11) NOT NULL,
  `id_kierowniklogistyki` int(11) NOT NULL,
  `id_magazyn` int(11) NOT NULL,
  `ilosc` int(11) NOT NULL,
  `typ` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id_sprzet`),
  KEY `id_kierowniklogistyki` (`id_kierowniklogistyki`),
  CONSTRAINT `sprzet_ibfk_1` FOREIGN KEY (`id_kierowniklogistyki`) REFERENCES `kierowniklogistyki` (`id_kierowniklogistyki`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `system` (
  `id_system` int(11) NOT NULL,
  `id_administrator` int(11) NOT NULL,
  PRIMARY KEY (`id_system`),
  KEY `id_administrator` (`id_administrator`),
  CONSTRAINT `system_ibfk_1` FOREIGN KEY (`id_administrator`) REFERENCES `administrator` (`id_administrator`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `zamowienie` (
  `id_zamowienie` int(11) NOT NULL,
  `id_kierowniklogistyki` int(11) NOT NULL,
  `data_wykonania` date DEFAULT NULL,
  `data_zalozenia` date NOT NULL,
  `dostawca` varchar(50) NOT NULL,
  `producent` varchar(50) NOT NULL,
  `rodzaj_sprzetu` varchar(50) NOT NULL,
  PRIMARY KEY (`id_zamowienie`),
  KEY `id_kierowniklogistyki` (`id_kierowniklogistyki`),
  CONSTRAINT `zamowienie_ibfk_1` FOREIGN KEY (`id_kierowniklogistyki`) REFERENCES `kierowniklogistyki` (`id_kierowniklogistyki`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `zadanie` (
  `id_zadanie` int(11) NOT NULL,
  `data_przydzielenia` date NOT NULL,
  `data_wykonania` date DEFAULT NULL,
  `priorytet` int(11) NOT NULL,
  `tresc` varchar(300) NOT NULL,
  `id_pracownikelektrowni` int(11) NOT NULL,
  PRIMARY KEY (`id_zadanie`),
  KEY `id_pracownikelektrowni` (`id_pracownikelektrowni`),
  CONSTRAINT `zadanie_ibfk_1` FOREIGN KEY (`id_pracownikelektrowni`) REFERENCES `pracownikelektrowni` (`id_pracownikelektrowni`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `zatrudnienie` (
  `id_zatrudnienie` int(11) NOT NULL,
  `data_wygasniecia_umowy` date DEFAULT NULL,
  `data_zawarcia_umowy` date NOT NULL,
  `placa` decimal(18,2) NOT NULL,
  `rodzaj_umowy` varchar(50) DEFAULT NULL,
  `stanowisko` varchar(50) DEFAULT NULL,
  `id_zatrudnienie_elektrownia_jadrowa` int(11) NOT NULL,
  `id_zatrudnienie_pracownika_elektrowni` int(11) NOT NULL,
  PRIMARY KEY (`id_zatrudnienie`),
  KEY `id_zatrudnienie_pracownika_elektrowni` (`id_zatrudnienie_pracownika_elektrowni`),
  CONSTRAINT `zatrudnienie_ibfk_1` FOREIGN KEY (`id_zatrudnienie_pracownika_elektrowni`) REFERENCES `pracownikelektrowni` (`id_pracownikelektrowni`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `zmiana` (
  `id_zmiana` int(11) NOT NULL,
  `id_pracownikelektrowni` int(11) NOT NULL,
  `id_grafik` int(11) NOT NULL,
  `data_godzina_konca` datetime NOT NULL,
  `data_godzina_rozpoczecia` datetime NOT NULL,
  PRIMARY KEY (`id_zmiana`),
  KEY `id_grafik` (`id_grafik`),
  KEY `id_pracownikelektrowni` (`id_pracownikelektrowni`),
  CONSTRAINT `zmiana_ibfk_1` FOREIGN KEY (`id_grafik`) REFERENCES `grafik` (`id_grafik`),
  CONSTRAINT `zmiana_ibfk_2` FOREIGN KEY (`id_pracownikelektrowni`) REFERENCES `pracownikelektrowni` (`id_pracownikelektrowni`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

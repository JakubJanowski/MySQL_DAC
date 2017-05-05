create database elektrownia
go

use elektrownia
go


create table Pracownik (
	PESEL varchar(11) primary key,
	imie varchar(100) not null,
	nazwisko varchar(100) not null,
	adres varchar(300),
	data_urodzenia datetime,
	wyksztalcenie varchar(8)
)
go

create table Reaktor (
	id int primary key,
	paliwo varchar(20) not null,
	moc decimal not null
)
go

create table Parametry_reaktora(
	reaktor_id int foreign key references Reaktor,
	czas datetime not null,
	wytworzona_energia decimal,
	temperatura decimal,
	cisnienie decimal,
	stan_paliwa decimal,
	primary key("reaktor_id", "czas")
)
go

create table Awaria (
	reaktor_id int,
	czas datetime,
	data_naprawienia date null,
	foreign key(reaktor_id, czas) references Parametry_reaktora(reaktor_id, czas),
	primary key(reaktor_id, czas)
)
go

create table Zatrudnienie (
	id int primary key,
	PESEL_pracownika varchar(11) foreign key references Pracownik,
	data_zatrudnienia date default getdate(),
	data_zwolnienia date default null,
	stanowisko varchar(30)
)
go

create table Jest_na_zmianie (
	reaktor_id int foreign key references Reaktor,
	czas datetime not null,
	zatrudnienie_id int foreign key references Zatrudnienie,
	primary key(reaktor_id, czas, zatrudnienie_id),
	foreign key(reaktor_id, czas) references Parametry_reaktora(reaktor_id, czas)
)
go

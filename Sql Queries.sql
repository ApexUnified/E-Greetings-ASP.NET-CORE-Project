create database E_Greetings;

use E_Greetings;

create table users (id int identity primary key, name varchar(255), email varchar(255) unique, password varchar(255), role_id int, CONSTRAINT fk_role_id FOREIGN KEY (role_id) REFERENCES roles(id));
create table roles (id int identity primary key, name varchar(255));
create table permissions(id int identity primary key, name varchar(255));
create table role_permissions(id int identity primary key, role_id int, CONSTRAINT fk_role_permission FOREIGN KEY (role_id) REFERENCES roles(id), permission_id int , CONSTRAINT fk_permission_id FOREIGN KEY (permission_id) REFERENCES permissions(id));
create table subscribers (id int identity, status int   ,user_id int null CONSTRAINT fk_subscriber FOREIGN KEY (user_id) REFERENCES users(id) ,is_guest int, email varchar(255) , ip_address varchar(255) null , fee varchar(255), subscription_start_date datetime, subscription_end_date datetime   )
create table cards_list (id int identity primary key, card_name varchar(255) ,controller varchar(255) , action varchar(255) )
create table cards (id int identity primary key, card_list_id int CONSTRAINT fk_cards FOREIGN KEY (card_list_id) REFERENCES cards_list(id), status int )
create table cards_sent(id int identity primary key, user_name varchar(255) null ,card_id int FOREIGN KEY (card_id) REFERENCES cards_list(id), sent_from TEXT, sent_to TEXT , sent_at datetime )
create table feedbacks(id int identity primary key, name varchar(255), email varchar(255), message TEXT, status int )
create table transactions (id int identity primary key, name varchar(255), money varchar(255), at datetime)

drop table subscribers
drop table cards_list
drop table cards
drop table cards_sent

truncate table subscribers

insert into roles (name) values('Admin'),('User')
insert into users (name , email, password, role_id)values('Abdullah','abdullahsheikhmuhammad21@gmail.com','12345678',1)
insert into permissions(name)values('HomeIndex');
insert into role_permissions(role_id,permission_id)values(1,1);
insert into cards_list(card_name, controller,action) values('Wedding Card','Website','Wedding'),('Anyversary Card','Website','Anyversary')
insert into cards_list(card_name, controller,action) values('Birthday Card','Website','Birthday')
insert into feedbacks(name, email, message, status)values('Abdullah','ab@gmail.com','dhsadhsakjd',1),('Abdullah','ab@gmail.com','dhsadhsakjd',1),('Abdullah','ab@gmail.com','dhsadhsakjd',1),('Abdullah','ab@gmail.com','dhsadhsakjd',1),('Abdullah','ab@gmail.com','dhsadhsakjd',1)
select * from users
select * from roles
select * from permissions
select * from role_permissions
select * from subscribers
select * from cards_list
select * from cards
select * from cards_sent
select * from feedbacks
select * from transactions



SELECT * FROM Subscribers WHERE Id = 3 AND is_guest = 1;

Update subscribers set status = 0 where id = 3 

delete from subscribers where id = 3

truncate table users
truncate table subscribers


ALTER TABLE users
DROP CONSTRAINT fk_role_id;

ALTER TABLE users
ADD CONSTRAINT fk_role_id FOREIGN KEY (role_id) REFERENCES roles(id)
ON UPDATE CASCADE ON DELETE CASCADE;


ALTER TABLE role_permissions
DROP CONSTRAINT fk_role_permission;

ALTER TABLE role_permissions
ADD CONSTRAINT fk_role_permission FOREIGN KEY (role_id) REFERENCES roles(id)
ON UPDATE CASCADE ON DELETE CASCADE;


ALTER TABLE subscribers
DROP CONSTRAINT fk_subscriber;

ALTER TABLE subscribers
ADD CONSTRAINT fk_subscriber FOREIGN KEY (user_id) REFERENCES users(id)
ON UPDATE CASCADE ON DELETE CASCADE;


ALTER TABLE cards
DROP CONSTRAINT fk_cards;

ALTER TABLE cards
ADD CONSTRAINT fk_cards FOREIGN KEY (card_list_id) REFERENCES cards_list(id)
ON UPDATE CASCADE ON DELETE CASCADE;


ALTER TABLE cards_sent
DROP CONSTRAINT fk_cards_send;

ALTER TABLE cards_sent
ADD CONSTRAINT fk_cards_send FOREIGN KEY (user_id) REFERENCES users(id)
ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE subscribers
ADD CONSTRAINT DF_UserId DEFAULT -1 FOR user_id;


select * from permissions
delete from role_permissions where permission_id = 19
delete from permissions where id = 39

update  permissions  set name = 'Edit Per' where id = 19



ALTER TABLE subscribers ADD created_at DATETIME DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE cards_sent ADD created_at DATETIME DEFAULT CURRENT_TIMESTAMP;

update cards_sent set created_at = CURRENT_TIMESTAMP
update subscribers set created_at = CURRENT_TIMESTAMP
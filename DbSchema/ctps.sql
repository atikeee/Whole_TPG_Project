-- --------------------------------------------------------
-- Host:                         sd-ct-opiot.sn.intel.com
-- Server version:               5.7.19 - MySQL Community Server (GPL)
-- Server OS:                    Linux
-- HeidiSQL Version:             9.4.0.5125
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Dumping structure for table ctpstestplandb.band_app_cri_table
DROP TABLE IF EXISTS `band_app_cri_table`;
CREATE TABLE IF NOT EXISTS `band_app_cri_table` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `bandapplicability` varchar(50) DEFAULT NULL,
  `bandcriteria` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- Dumping data for table ctpstestplandb.band_app_cri_table: ~6 rows (approximately)
/*!40000 ALTER TABLE `band_app_cri_table` DISABLE KEYS */;
REPLACE INTO `band_app_cri_table` (`id`, `bandapplicability`, `bandcriteria`) VALUES
	(1, 'Single', 'C1'),
	(2, 'ALL', 'C2'),
	(3, 'IRAT-Single', 'C1'),
	(4, 'IRAT-All', 'C2'),
	(5, 'RAT-All', 'C2'),
	(6, '', '');
/*!40000 ALTER TABLE `band_app_cri_table` ENABLE KEYS */;

-- Dumping structure for procedure ctpstestplandb.deletealldata
DROP PROCEDURE IF EXISTS `deletealldata`;
DELIMITER //
CREATE DEFINER=`su`@`%` PROCEDURE `deletealldata`()
    COMMENT 'Be Carefule'
BEGIN
delete  from mapping_tc_band_table;
delete from tcdata;
delete from testcasetable;
delete from testbandconfig;
delete from gcfptcrbver;
delete from user_picsmappingtable;
delete from user_picsstatus;
delete from user_picsver;
END//
DELIMITER ;

-- Dumping structure for procedure ctpstestplandb.deleteallserverdata
DROP PROCEDURE IF EXISTS `deleteallserverdata`;
DELIMITER //
CREATE DEFINER=`su`@`%` PROCEDURE `deleteallserverdata`()
    COMMENT 'Be Carefule'
BEGIN
delete  from mapping_tc_band_table;
delete from tcdata;
delete from testcasetable;
delete from testbandconfig;
delete from gcfptcrbver;
END//
DELIMITER ;

-- Dumping structure for procedure ctpstestplandb.deletetcinfo
DROP PROCEDURE IF EXISTS `deletetcinfo`;
DELIMITER //
CREATE DEFINER=`su`@`%` PROCEDURE `deletetcinfo`(
	IN `v` INT


)
BEGIN
delete  from mapping_tc_band_table where  `id#ver` = v;
-- delete from tcdata;
-- delete from testcasetable;
-- delete from testbandconfig;
delete from gcfptcrbver where `id` = v;
END//
DELIMITER ;

-- Dumping structure for procedure ctpstestplandb.deletever
DROP PROCEDURE IF EXISTS `deletever`;
DELIMITER //
CREATE DEFINER=`su`@`%` PROCEDURE `deletever`(
	IN `v` VARCHAR(50)
)
BEGIN
delete from mapping where ver = v;
delete from tcdatatable where ID not in (select distinct DATAID from mapping);
delete from tcconfigtable where ID not in (select distinct F_ID from tcdatatable);
END//
DELIMITER ;

-- Dumping structure for procedure ctpstestplandb.delete_picsdata
DROP PROCEDURE IF EXISTS `delete_picsdata`;
DELIMITER //
CREATE DEFINER=`su`@`%` PROCEDURE `delete_picsdata`(
	IN `gv1` VARCHAR(200)
,
	IN `pv2` VARCHAR(200)





)
BEGIN
delete from user_picsmappingtable where `id#pics` in (select id from user_picsver where  (`gcfver` = gv1 and `picsver` = pv2));
delete from user_picsver where (`gcfver` = gv1 and `picsver` = pv2);
-- select id from user_picsver where  (`gcfver` = gv1 and `picsver` = pv2);
END//
DELIMITER ;

-- Dumping structure for table ctpstestplandb.envcond
DROP TABLE IF EXISTS `envcond`;
CREATE TABLE IF NOT EXISTS `envcond` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `envcond` varchar(40) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- Dumping data for table ctpstestplandb.envcond: ~4 rows (approximately)
/*!40000 ALTER TABLE `envcond` DISABLE KEYS */;
REPLACE INTO `envcond` (`id`, `envcond`) VALUES
	(1, 'NC'),
	(2, 'NC,TL/VL,TL/VH,TH/VL,TH/VH'),
	(3, 'TBD'),
	(6, '');
/*!40000 ALTER TABLE `envcond` ENABLE KEYS */;

-- Dumping structure for table ctpstestplandb.gcfptcrbver
DROP TABLE IF EXISTS `gcfptcrbver`;
CREATE TABLE IF NOT EXISTS `gcfptcrbver` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ver_gcf_ptcrb_op` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=112 DEFAULT CHARSET=utf8;

-- Dumping data for table ctpstestplandb.gcfptcrbver: ~2 rows (approximately)
/*!40000 ALTER TABLE `gcfptcrbver` DISABLE KEYS */;
/*!40000 ALTER TABLE `gcfptcrbver` ENABLE KEYS */;

-- Dumping structure for procedure ctpstestplandb.getcombinedtestplan
DROP PROCEDURE IF EXISTS `getcombinedtestplan`;
DELIMITER //
CREATE DEFINER=`su`@`%` PROCEDURE `getcombinedtestplan`(
	IN `ver1` INT,
	IN `ver2` INT






)
BEGIN               
-- select * from (
-- select `GCF/PTCRB/Operator Version`,`PICS Version`,`Spec`,SheetName,`Test Case Number`,Band,`Band Applicability`,`Band Criteria`,`Certified All TP`, `PICS Status`, `Env_Cond`, `Band Support`, `ICE Band Support`,`Required Bands` from (
-- (select `id#tcconf`,`id#bandconfig`,`id#banddata`,`id#tcdata` ,`GCF/PTCRB/Operator Version`,`PICS Version`,`Spec`,SheetName,`Test Case Number`,Band,`Band Applicability`,`Band Criteria`,`Certified All TP`, `PICS Status`, `Env_Cond`, `Band Support`, `ICE Band Support`,`Required Bands` from v where `id_ver`=v1)
select Spec,Sheetname,`Test Case Number`, Description,Band,Env_Cond,`TC Status` as tcstatus_gcf,'' as tcstatus_ptcrb,`Cert TP [V]` as TP_gcf_V,'' as TP_ptcrb_V,`Cert TP [E]` as TP_gcf_E,'' as TP_ptcrb_E,`Cert TP [D]` as TP_gcf_D,'' as TP_ptcrb_D,`PICS Status` as pics_status_gcf,''as pics_status_ptcrb,`Band Support`, `ICE Band Support` from v where id_ver = ver1

and (
(`id#tcconf`,`id#bandconfig`) not in 

(select* from (
((select `id#tcconf`,`id#bandconfig` from v where `id_ver`=ver1)
union all
(select `id#tcconf`,`id#bandconfig` from v where `id_ver`=ver2))
 `vv`)
               GROUP  BY `id#tcconf`,`id#bandconfig`
               HAVING COUNT(*) = 2 
)

)
union all

select Spec,SheetName,`Test Case Number`, Description,Band,Env_Cond,'',`TC Status`,'',`Cert TP [V]`,'',`Cert TP [E]`,'',`Cert TP [D]`,'',`PICS Status`,`Band Support`, `ICE Band Support` from v where id_ver = ver2

and (
(`id#tcconf`,`id#bandconfig`) not in 

(select* from (
((select `id#tcconf`,`id#bandconfig` from v where `id_ver`=ver1)
union all
(select `id#tcconf`,`id#bandconfig` from v where `id_ver`=ver2))
 `vv`)
               GROUP  BY `id#tcconf`,`id#bandconfig` 
               HAVING COUNT(*) = 2 
)

)
union all
select v1.Spec,v1.SheetName,v1.`Test Case Number`,v1.Description,v1.`Band`,v1.Env_Cond,v1.`TC Status`,v2.`TC Status`,v1.`Cert TP [V]`,v2.`Cert TP [V]`,v1.`Cert TP [E]`,v2.`Cert TP [E]`,v1.`Cert TP [D]`,v2.`Cert TP [D]`,v1.`PICS Status`,v2.`PICS Status`,v1.`Band Support`, v1.`ICE Band Support` from (
(select * from v where id_ver = ver1) v1,(select * from v where id_ver = ver2) v2)
where v1.`id#tcconf`=v2.`id#tcconf` and v1.`id#bandconfig`= v2.`id#bandconfig`;
END//
DELIMITER ;

-- Dumping structure for procedure ctpstestplandb.getdeltaofcombverpics
DROP PROCEDURE IF EXISTS `getdeltaofcombverpics`;
DELIMITER //
CREATE DEFINER=`su`@`%` PROCEDURE `getdeltaofcombverpics`(
	IN `v1` VARCHAR(50),
	IN `v2` VARCHAR(50)

)
BEGIN
select distinct `v1`.`SheetName` AS `Category`,`v1`.`Standards` AS `Standards`,`v1`.`TCNumber` AS `TCNumber`,`v1`.`Band` AS `Band`,
-- `v1`.`VER` AS `VER1`,"" AS `VER2` ,
-- `v1`.`ID` AS `ID1`,""AS `ID2`,
-- `v1`.`PICS_Ver` AS `PICS_Ver1`,"" AS `PICS_Ver2`,
`v1`.`GCF_Certified_All_TP` AS `GCF_Cert_All_TP(V1)`,""AS `GCF_Cert_All_TP(V2)`,
`v1`.`TC_Status` AS `TC_Stat(V1)`,""AS `TC_Stat(V2)`,
`v1`.`PICS_Status` AS `PICS_Stat(V1)`,""AS `PICS_Stat(V2)`,
`v1`.`Env_Cond` AS `Env_Cond(V1)`,""AS `Env_Cond(V2)`,
`v1`.`Band_Support` AS `Band_Supp(V1)`,""AS `Band_Supp(V1)`,
`v1`.`ICE_Band_Support` AS `ICE_Band_Supp(V1)`,""AS `ICE_Band_Supp(V2)`
from((select * from v where combverpics = v1) `v1` join(select * from v where combverpics = v2) `v2`) where `v1`.`F_ID` not in (select F_ID from v where combverpics = v2)
UNION
select distinct `v2`.`SheetName` AS `Category`,`v2`.`Standards` AS `Standards`,`v2`.`TCNumber` AS `TCNumber`,`v2`.`Band` AS `Band`,
-- "" AS `VER1` ,`v2`.`VER` AS `VER2`,
-- "" AS `ID1`,`v2`.`ID` AS `ID2`,
-- "" AS `PICS_Ver1`,`v2`.`PICS_Ver` AS `PICS_Ver2`,
"" AS `GCF_Cert_All_TP(V1)`,`v2`.`GCF_Certified_All_TP` AS `GCF_Cert_All_TP(V2)`,
"" AS `TC_Stat(V1)`,`v2`.`TC_Status` AS `TC_Stat(V2)`,
"" AS `PICS_Stat(V1)`,`v2`.`PICS_Status` AS `PICS_Stat(V2)`,
"" AS `Env_Cond(V1)`,`v2`.`Env_Cond` AS `Env_Cond(V2)`,
"" AS `Band_Supp(V1)`,`v2`.`Band_Support` AS `Band_Supp(V2)`,
"" AS `ICE_Band_Supp(V1)`,`v2`.`ICE_Band_Support` AS `ICE_Band_Supp(V2)`
from((select * from v where combverpics = v1) `v1` join(select * from v where combverpics = v2) `v2`) where `v2`.`F_ID` not in (select F_ID from v where combverpics = v1)
UNION
select distinct `v1`.`SheetName` AS `Category`,`v1`.`Standards` AS `Standards`,`v1`.`TCNumber` AS `TCNumber`,`v1`.`Band` AS `Band`,
-- `v1`.`VER` AS `VER1`,`v2`.`VER` AS `VER2`,
-- `v1`.`ID` AS `ID1`,`v2`.`ID` AS `ID2`,
-- `v1`.`PICS_Ver` AS `PICS_Ver1`,`v2`.`PICS_Ver` AS `PICS_Ver2`,
`v1`.`GCF_Certified_All_TP` AS `GCF_Cert_All_TP(V1)`,`v2`.`GCF_Certified_All_TP` AS `GCF_Cert_All_TP(V2)`,
`v1`.`TC_Status` AS `TC_Stat(V1)`,`v2`.`TC_Status` AS `TC_Stat(V2)`,
`v1`.`PICS_Status` AS `PICS_Stat(V1)`,`v2`.`PICS_Status` AS `PICS_Stat(V2)`,
`v1`.`Env_Cond` AS `Env_Cond(V1)`,`v2`.`Env_Cond` AS `Env_Cond(V2)`,
`v1`.`Band_Support` AS `Band_Supp(V1)`,`v2`.`Band_Support` AS `Band_Supp(V2)`,
`v1`.`ICE_Band_Support` AS `ICE_Band_Supp(V1)`,`v2`.`ICE_Band_Support` AS `ICE_Band_Supp(V2)`
from((select * from v where combverpics = v1) `v1` join(select * from v where combverpics = v2) `v2`) where((`v1`.`F_ID` = `v2`.`F_ID`) and (`v1`.`ID` <> `v2`.`ID`));
END//
DELIMITER ;

-- Dumping structure for procedure ctpstestplandb.getdeltaofver
DROP PROCEDURE IF EXISTS `getdeltaofver`;
DELIMITER //
CREATE DEFINER=`su`@`%` PROCEDURE `getdeltaofver`(
	IN `v1` INT,
	IN `v2` INT





)
BEGIN               
-- select * from (
-- select `GCF/PTCRB/Operator Version`,`PICS Version`,`Spec`,SheetName,`Test Case Number`,Band,`Band Applicability`,`Band Criteria`,`Certified All TP`, `PICS Status`, `Env_Cond`, `Band Support`, `ICE Band Support`,`Required Bands` from (
-- (select `id#tcconf`,`id#bandconfig`,`id#banddata`,`id#tcdata` ,`GCF/PTCRB/Operator Version`,`PICS Version`,`Spec`,SheetName,`Test Case Number`,Band,`Band Applicability`,`Band Criteria`,`Certified All TP`, `PICS Status`, `Env_Cond`, `Band Support`, `ICE Band Support`,`Required Bands` from v where `id_ver`=v1)
select `GCF/PTCRB/Operator Version`,`PICS Version`,`Spec`,SheetName,`Test Case Number`,Band, `TC Status`, `Band Applicability`,`Band Criteria`,`Cert TP [V]`,`Cert TP [E]`,`Cert TP [D]`, `PICS Status`, `Env_Cond`, `Band Support`, `ICE Band Support`,`Required Bands` from (
(select `id#tcconf`,`id#bandconfig`,`id#tcdata` ,`id#user_picsstatus`, `TC Status` , `id#bsrb`,id_ver,`GCF/PTCRB/Operator Version`,`PICS Version`,`Spec`,SheetName,`Test Case Number`,Band,`Band Applicability`,`Band Criteria`,`Cert TP [V]`, `Cert TP [E]`,`Cert TP [D]`,`PICS Status`, `Env_Cond`, `Band Support`, `ICE Band Support`,`Required Bands` from v where `id_ver`=v1)
union all
(select `id#tcconf`,`id#bandconfig`,`id#tcdata`,`id#user_picsstatus` ,  `TC Status` ,`id#bsrb`,id_ver,`GCF/PTCRB/Operator Version`,`PICS Version`,`Spec`,SheetName,`Test Case Number`,Band,`Band Applicability`,`Band Criteria`,`Cert TP [V]`, `Cert TP [E]`, `Cert TP [D]`,`PICS Status`, `Env_Cond`, `Band Support`, `ICE Band Support`,`Required Bands`  from v where `id_ver`=v2))
 `vv`
group by `id#tcconf`,`id#bandconfig`,`id#tcdata`,`id#user_picsstatus`, `id#bsrb`,`TC Status`
having count(*) =1;
END//
DELIMITER ;

-- Dumping structure for table ctpstestplandb.iceband
DROP TABLE IF EXISTS `iceband`;
CREATE TABLE IF NOT EXISTS `iceband` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `iceproj` varchar(50) NOT NULL,
  `bandlist` text NOT NULL,
  `bandlist_ulca` text,
  `4x4_mimo` text,
  PRIMARY KEY (`iceproj`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- Dumping data for table ctpstestplandb.iceband: ~2 rows (approximately)
/*!40000 ALTER TABLE `iceband` DISABLE KEYS */;
REPLACE INTO `iceband` (`id`, `iceproj`, `bandlist`, `bandlist_ulca`, `4x4_mimo`) VALUES
	(1, '7480', '1 2 3 4 5 7 8 12 13 17 18 19 20 25 26 28 29 30 34 38 39 40 41 66 7C 38C 40C 41C 2C 3C 7B 7C 12B 38C 39C 40C 41C 41D 66B 66C 2A-2A 3A-3A 4A-4A 7A-7A 25A-25A 41A-41A 41A-41C 41C-41A 66A-66A 1A-3A 1A-3C 1A-5A 1A-7A 1A-8A 1A-18A 1A-19A 1A-20A 1A-26A 1A-28A 2A-2A-5A 2A-2A-12A 2A-2A-13A 2A-2A-29A 2A-2A-30A 2A-4A 2A-5A 2A-12A 2C-12A 2A-12B 2A-13A 2A-17A 2A-29A 2A-30A 2C-30A 2A-66A 2A-66B 2A-66C 3A-3A-7A 3A-3A-7A-7A 3A-3A-8A 3A-5A 3C-5A 3A-7A 3A-7A-7A 3A-7B 3A-7C 3C-7A 3A-8A 3A-19A 3A-20A 3A-26A 3A-28A 4A-4A-5A 4A-4A-7A 4A-4A-12A 4A-4A-13A 4A-4A-29A 4A-4A-30A 4A-5A 4A-7A 4A-12A 4A-12B 4A-13A 4A-17A 4A-29A 4A-30A 5A-7A 5A-25A 5A-30A 5A-40A 5A-40C 7A-8A 7A-12A 7A-20A 7A-28A 7B-28A 7C-28A 12A-30A 12A-66A 12A-66A-66A 12A-66B 13A-66A 20A-38A 25A-26A 29A-30A 39A-41A 39C-41A 39A-41C 1A-3A-5A 1A-3A-7A 1A-3A-8A 1A-3A-19A 1A-3A-20A 1A-3A-26A 1A-3A-28A 1A-5A-7A 1A-7A-8A 1A-7A-20A 1A-7A-28A 2A-2A-5A-30A 2A-2A-12A-30A 2A-2A-29A-30A 2C-29A-30A 2A-4A-5A 2A-4A-12A 2A-4A-13A 2A-4A-29A 2A-4A-30A 2A-5A-30A 2C-5A-30A 2A-12A-30A 2C-12A-30A 2A-12A-66A 2A-29A-30A 3A-7A-8A 3A-7A-20A 3A-7A-28A 3A-7C-28A 4A-4A-5A-30A 4A-4A-12A-30A 4A-4A-29A-30A 4A-5A-30A 4A-7A-12A 4A-12A-30A 4A-29A-30A 1A-3A-7A-8A 1A-3A-7A-28A 2A-4A-5A-30A 2A-4A-12A-30A 2A-4A-29A-30A I II IV V VI VIII 850 900 1800 1900 A E F', '7C 38C 40C 41C', NULL),
	(2, '7560', '1 2 3 4 5 7 8 11 12 13 14 17 18 19 20 21 25 26 28 29 30 32 34 38 39 40 41 42 46 66 71 1C 2C 3C 5B 7B 7C 12B 38C 39C 40C 40D 40E 41C 41D 41E 42C 46C 46D 46E 66B 66C 66D 1A-1A 2A-2A 3A-3A 4A-4A 7A-7A 25A-25A 40A-40A 41A-41A 41A-41C 41A-41C 41A-41D 41C-41C 41C-41D 46A-46A 46A-46C 46A-46D 66A-66A 66A-66B 66A-66C 1A-1A-3A 1A-1A-3C 1A-1A-5A 1A-1A-28A 1A-3A 1A-3A 1A-3A-3A 1A-3A-3A 1A-3C 1A-3C 1A-5A 1A-7A 1A-7A 1A-7C 1A-7C 1A-8A 1A-18A 1A-19A 1A-20A 1A-26A 1A-28A 1A-38A 1A-38A 1A-40A 1A-40A 1A-40C 1A-40C 1A-41A 1A-41A 1A-41C 1A-41C 1A-42A 1A-42A 1A-42C 1A-42C 1A-46A 1A-46C 1A-46D 1C-3A 1C-3A 1C-41A 1C-41A 2A-2A-4A 2A-2A-4A-4A 2A-2A-5A 2A-2A-12A 2A-2A-12B 2A-2A-13A 2A-2A-14A 2A-2A-29A 2A-2A-30A 2A-2A-46A 2A-2A-46C 2A-2A-46D 2A-2A-66A 2A-2A-66A-66A 2A-2A-66A-66B 2A-2A-66A-66C 2A-2A-66B 2A-2A-66C 2A-2A-71A 2A-4A 2A-4A 2A-4A-4A 2A-5A 2A-5B 2A-12A 2A-12B 2A-13A 2A-14A 2A-17A 2A-28A 2A-29A 2A-30A 2A-30A 2A-46A 2A-46A-46A 2A-46A-46C 2A-46A-46D 2A-46C 2A-46D 2A-46E 2A-66A 2A-66A 2A-66B 2A-66B 2A-66C 2A-66C 2A-66A-66A 2A-66A-66B 2A-66A-66C 2A-71A 2C-5A 2C-12A 2C-29A 2C-30A 2C-30A 2C-66A 2C-66A 3A-3A-7A 3A-3A-7A-7A 3A-3A-8A 3A-5A 3A-7A 3A-7A 3A-7A-7A 3A-7B 3A-7B 3A-7C 3A-7C 3A-8A 3A-19A 3A-20A 3A-26A 3A-28A 3A-38A 3A-38A 3A-40A 3A-40A 3A-40A-40A 3A-40C 3A-40C 3A-40D 3A-40E 3A-41A 3A-41A 3A-41C 3A-41C 3A-41D 3A-42A 3A-42A 3A-42C 3A-42C 3A-46A 3A-46C 3A-46D 3A-46E 3C-5A 3C-7A 3C-7A 3C-7C 3C-8A 3C-20A 3C-28A 3C-40A 3C-40A 3C-40C 3C-41A 3C-41A 3C-41C 3C-41D 4A-4A-5A 4A-4A-7A 4A-4A-12A 4A-4A-12B 4A-4A-13A 4A-4A-29A 4A-4A-30A 4A-4A-71A 4A-5A 4A-7A 4A-7A 4A-7A-7A 4A-12A 4A-12B 4A-13A 4A-17A 4A-28A 4A-29A 4A-30A 4A-30A 4A-46A 4A-46A-46A 4A-46A-46C 4A-46A-46D 4A-46C 4A-46D 4A-71A 5A-7A 5A-25A 5A-30A 5A-40A 5A-41A 5A-46A 5A-46C 5A-46D 5A-66A 5A-66B 5A-66C 5A-66D 5A-66A-66A 5A-66A-66B 5A-66A-66C 5B-30A 5B-66A 5B-66A-66A 7A-7A-8A 7A-8A 7A-12A 7A-20A 7A-28A 7A-40A 7A-40A 7A-40C 7A-40C 7A-40D 7A-42A 7A-42A 7A-46A 7A-46C 7A-46D 7A-66A 7A-66A 7B-28A 7C-28A 8A-11A 8A-39A 8A-39C 8A-40A 8A-41A 8A-41C 8A-41D 11A-18A 12A-30A 12A-66A 12A-66A-66A 12A-66C 13A-46A 13A-46C 13A-46D 13A-46E 13A-66A 13A-66A-66A 13A-66A-66B 13A-66A-66C 13A-66B 13A-66C 14A-30A 14A-66A 14A-66A-66A 19A-21A 19A-42A 19A-42C 20A-32A 20A-38A 20A-42A 21A-42A 21A-42C 25A-26A 25A-25A-26A 28A-40A 28A-40C 28A-40D 28A-41A 28A-41C 28A-42A 28A-42C 29A-30A 29A-66A 29A-66A-66A 30A-66A 30A-66A 30A-66A-66A 39A-41A 39A-41A 39A-41C 39A-41C 39A-41D 39C-41A 39C-41A 39C-41C 39C-41D 40A-46A 40A-46C 41A-42A 41A-42A 41A-42C 41A-42C 41C-42A 41C-42A 41C-42C 46A-46A-66A 46A-46C-66A 46A-46D-66A 46A-66A 46A-66A-66A 46C-66A 46C-66A-66A 46D-66A 46D-66A-66A 46E-66A 66A-66A-71A 66A-71A 66C-71A 1A-1A-3A-5A 1A-1A-3A-28A 1A-3A-5A 1A-3A-5A 1A-3A-7A 1A-3A-7A 1A-3A-7A 1A-3A-7C 1A-3A-7C 1A-3A-8A 1A-3A-8A 1A-3A-19A 1A-3A-19A 1A-3A-20A 1A-3A-20A 1A-3A-26A 1A-3A-26A 1A-3A-28A 1A-3A-28A 1A-3A-40A 1A-3A-40A 1A-3A-40A 1A-3A-40C 1A-3A-40C 1A-3A-41A 1A-3A-41A 1A-3A-41A 1A-3A-42A 1A-3A-42C 1A-3C-5A 1A-3C-7A 1A-3C-7A 1A-3C-7C 1A-3C-8A 1A-3C-28A 1A-3C-41A 1A-3C-41A 1A-5A-7A 1A-5A-40A 1A-7A-8A 1A-7A-20A 1A-7A-28A 1A-7A-46A 1A-7A-46A 1A-7A-46C 1A-7A-46C 1A-7A-46D 1A-7C-28A 1A-8A-40A 1A-19A-42A 1A-19A-42C 1A-41A-42A 1A-41A-42A 1A-41A-42C 1A-41A-42C 1A-41C-42A 1A-41C-42C 1C-3A-41A 1C-3A-41A 2A-2A-4A-5A 2A-2A-4A-12A 2A-2A-5A-30A 2A-2A-5A-66A 2A-2A-5A-66C 2A-2A-12A-30A 2A-2A-12A-66A 2A-2A-13A-66A 2A-2A-14A-66A 2A-2A-29A-30A 2A-2A-30A-66A 2A-4A-4A-5A 2A-4A-4A-12A 2A-4A-5A 2A-4A-5A 2A-4A-12A 2A-4A-12A 2A-4A-12B 2A-4A-12B 2A-4A-13A 2A-4A-13A 2A-4A-29A 2A-4A-29A 2A-4A-30A 2A-4A-30A 2A-4A-30A 2A-4A-71A 2A-4A-71A 2A-5A-30A 2A-5A-66A 2A-5A-66A 2A-5A-66B 2A-5A-66C 2A-5A-66A-66A 2A-5B-30A 2A-5B-66A 2A-5B-66A 2A-5B-66A-66A 2A-12A-30A 2A-12A-66A 2A-12A-66A 2A-12A-66A-66A 2A-12A-66C 2A-13A-66A 2A-13A-66A 2A-13A-66A-66A 2A-13A-66B 2A-13A-66C 2A-14A-30A 2A-14A-66A 2A-14A-66A 2A-14A-66A-66A 2A-29A-30A 2A-29A-66A 2A-29A-66A 2A-30A-66A 2A-30A-66A 2A-30A-66A 2A-46A-66A 2A-46A-66A 2A-46C-66A 2A-46C-66A 2A-46D-66A 2A-66A-71A 2A-66A-71A 3A-5A-7A 3A-5A-40A 3A-7A-7A-8A 3A-7A-8A 3A-7A-20A 3A-7A-28A 3A-7A-40A 3A-7A-40A 3A-7A-40C 3A-7A-42A 3A-7A-42A 3A-7C-28A 3A-8A-40A 3A-19A-42A 3A-19A-42C 3A-20A-42A 3A-28A-40A 3A-28A-40C 3A-28A-40D 3A-28A-41A 3A-28A-41C 3A-28A-42A 3A-28A-42C 3A-41A-42A 3A-41A-42A 3A-41A-42C 3A-41A-42C 3A-41C-42A 3A-41C-42C 3C-7A-20A 3C-7A-28A 3C-7C-28A 4A-4A-5A-30A 4A-4A-12A-30A 4A-5A-30A 4A-7A-12A 4A-12A-30A 4A-29A-30A 5A-30A-66A 5A-30A-66A-66A 5B-30A-66A 5B-30A-66A-66A 7A-20A-42A 12A-30A-66A 12A-30A-66A-66A 13A-46A-66A 13A-46C-66A 13A-46D-66A 14A-30A-66A 14A-30A-66A-66A 19A-21A-42A 19A-21A-42C 28A-41A-42A 28A-41A-42C 28A-41C-42A 28A-41C-42C 29A-30A-66A 1A-3A-5A-7A 1A-3A-5A-40A 1A-3A-7A-8A 1A-3A-7A-20A 1A-3A-7A-28A 1A-3A-7C-28A 1A-3A-19A-42A 1A-3A-19A-42C 2A-4A-5A-30A 2A-4A-12A-30A 2A-4A-29A-30A 2A-5A-30A-66A 2A-5B-30A-66A 2A-12A-30A-66A 2A-14A-30A-66A 2A-29A-30A-66A I II IV V VI VIII 850 900 1800 1900 A E F', '7C 38C 39C 40C 41C', '1 2 3 4 7 25 30 34 38 39 40 41 66 1C 2C 3C 7B 7C 38C 39C 40C 41C 42C 66B 66C 1A-1A 2A-2A 3A-3A 4A-4A 7A-7A 25A-25A 40A-40A 41A-41A 41A-41C 41A-41C 41A-41D 66A-66A 66A-66B 66A-66C 1A-1A-3A 1A-1A-3C 1A-1A-5A 1A-3A 1A-3A 1A-3A-3A 1A-3A-3A 1A-3C 1A-3C 1A-5A 1A-7A 1A-7A 1A-7C 1A-7C 1A-8A 1A-18A 1A-19A 1A-20A 1A-26A 1A-28A 1A-38A 1A-38A 1A-40A 1A-40A 1A-40C 1A-40C 1A-41A 1A-41A 1A-41C 1A-41C 1A-42A 1A-42A 1A-42C 1A-42C 1A-46A 1A-46C 1A-46D 1C-3A 1C-3A 1C-41A 1C-41A 2A-2A-4A 2A-2A-5A 2A-2A-12A 2A-2A-12B 2A-2A-13A 2A-2A-14A 2A-2A-29A 2A-2A-30A 2A-2A-46A 2A-2A-46C 2A-2A-66A 2A-2A-66B 2A-2A-66C 2A-2A-71A 2A-4A 2A-4A 2A-4A-4A 2A-5A 2A-5B 2A-12A 2A-12B 2A-13A 2A-14A 2A-17A 2A-28A 2A-29A 2A-30A 2A-30A 2A-46A 2A-46C 2A-46D 2A-66A 2A-66A 2A-66B 2A-66B 2A-66C 2A-66C 2A-66A-66A 2A-66A-66B 2A-66A-66C 2A-71A 2C-5A 2C-12A 2C-29A 2C-30A 2C-30A 2C-66A 2C-66A 3A-3A-7A 3A-3A-8A 3A-5A 3A-7A 3A-7A 3A-7A-7A 3A-7B 3A-7B 3A-7C 3A-7C 3A-8A 3A-19A 3A-20A 3A-26A 3A-28A 3A-38A 3A-38A 3A-40A 3A-40A 3A-40A-40A 3A-40C 3A-40C 3A-40D 3A-41A 3A-41A 3A-41C 3A-41C 3A-41D 3A-42A 3A-42A 3A-42C 3A-42C 3A-46A 3A-46C 3A-46D 3C-5A 3C-7A 3C-7A 3C-8A 3C-20A 3C-28A 3C-40A 3C-40A 3C-41A 3C-41A 4A-4A-5A 4A-4A-7A 4A-4A-12A 4A-4A-12B 4A-4A-13A 4A-4A-29A 4A-4A-30A 4A-4A-71A 4A-5A 4A-7A 4A-7A 4A-7A-7A 4A-12A 4A-12B 4A-13A 4A-17A 4A-28A 4A-29A 4A-30A 4A-30A 4A-46A 4A-46C 4A-46D 4A-71A 5A-7A 5A-25A 5A-30A 5A-40A 5A-41A 5A-66A 5A-66B 5A-66C 5A-66A-66A 5A-66A-66B 5A-66A-66C 5B-30A 5B-66A 5B-66A-66A 7A-8A 7A-12A 7A-20A 7A-28A 7A-40A 7A-40A 7A-40C 7A-40C 7A-40D 7A-42A 7A-42A 7A-46A 7A-46C 7A-46D 7A-66A 7A-66A 7B-28A 7C-28A 8A-39A 8A-39C 8A-40A 8A-41A 8A-41C 12A-30A 12A-66A 12A-66A-66A 12A-66C 13A-66A 13A-66A-66A 13A-66A-66B 13A-66A-66C 13A-66B 13A-66C 14A-30A 14A-66A 14A-66A-66A 19A-42A 19A-42C 20A-38A 20A-42A 25A-26A 25A-25A-26A 28A-40A 28A-40C 28A-41A 28A-41C 28A-42A 28A-42C 29A-30A 29A-66A 29A-66A-66A 30A-66A 30A-66A 30A-66A-66A 39A-41A 39A-41A 39A-41C 39A-41C 39A-41D 39C-41A 39C-41A 40A-46A 40A-46C 41A-42A 41A-42A 41A-42C 41A-42C 41C-42A 41C-42A 46A-66A 46A-66A-66A 46C-66A 46C-66A-66A 46D-66A 66A-66A-71A 66A-71A 66C-71A 1A-3A-5A 1A-3A-5A 1A-3A-7A 1A-3A-7A 1A-3A-7A 1A-3A-7C 1A-3A-7C 1A-3A-8A 1A-3A-8A 1A-3A-19A 1A-3A-19A 1A-3A-20A 1A-3A-20A 1A-3A-26A 1A-3A-26A 1A-3A-28A 1A-3A-28A 1A-3A-40A 1A-3A-40A 1A-3A-40A 1A-3A-40C 1A-3A-40C 1A-3A-41A 1A-3A-41A 1A-3A-41A 1A-3A-42A 1A-3A-42C 1A-3C-5A 1A-3C-7A 1A-3C-7A 1A-3C-8A 1A-3C-28A 1A-3C-41A 1A-3C-41A 1A-5A-7A 1A-5A-40A 1A-7A-8A 1A-7A-20A 1A-7A-28A 1A-7A-46A 1A-7A-46A 1A-7A-46C 1A-7A-46C 1A-8A-40A 1A-19A-42A 1A-19A-42C 1A-41A-42A 1A-41A-42A 1A-41A-42C 1A-41A-42C 1A-41C-42A 1C-3A-41A 1C-3A-41A 2A-4A-5A 2A-4A-5A 2A-4A-12A 2A-4A-12A 2A-4A-12B 2A-4A-12B 2A-4A-13A 2A-4A-13A 2A-4A-29A 2A-4A-29A 2A-4A-30A 2A-4A-30A 2A-4A-30A 2A-4A-71A 2A-4A-71A 2A-5A-30A 2A-5A-66A 2A-5A-66A 2A-5A-66B 2A-5A-66C 2A-5B-30A 2A-5B-66A 2A-5B-66A 2A-12A-30A 2A-12A-66A 2A-12A-66A 2A-12A-66C 2A-13A-66A 2A-13A-66A 2A-13A-66B 2A-13A-66C 2A-14A-30A 2A-14A-66A 2A-14A-66A 2A-29A-30A 2A-29A-66A 2A-29A-66A 2A-30A-66A 2A-30A-66A 2A-30A-66A 2A-46A-66A 2A-46A-66A 2A-46C-66A 2A-46C-66A 2A-66A-71A 2A-66A-71A 3A-5A-7A 3A-5A-40A 3A-7A-8A 3A-7A-20A 3A-7A-28A 3A-7A-40A 3A-7A-40A 3A-7A-40C 3A-7A-42A 3A-7A-42A 3A-8A-40A 3A-19A-42A 3A-19A-42C 3A-20A-42A 3A-28A-40A 3A-28A-41A 3A-28A-42A 3A-28A-42C 3A-41A-42A 3A-41A-42A 3A-41A-42C 3A-41A-42C 3A-41C-42A 3C-7A-20A 3C-7A-28A 4A-5A-30A 4A-7A-12A 4A-12A-30A 4A-29A-30A 5A-30A-66A 5B-30A-66A 7A-20A-42A 12A-30A-66A 13A-46A-66A 13A-46C-66A 14A-30A-66A 28A-41A-42A 28A-41A-42C 29A-30A-66A'),
	(3, '7660', '1 2 3 4 5 7 8 11 12 13 14 17 18 19 20 21 25 26 28 29 30 32 34 38 39 40 41 42 46 48 49 66 71 1C 2C 3C 5B 7B 7C 7D 8B 12B 38C 39C 40C 40D 40E 41C 41D 41E 42C 42D 42E 46C 46D 46E 48C 48D 66B 66C 66D 1A-1A 2A-2A 3A-3A 4A-4A 7A-7A 25A-25A 40A-40A 41A-41A 41A-41C 41A-41D 41C-41C 41C-41D 46A-46A 46A-46C 46A-46D 48A-48A 48A-48C 66A-66A 66A-66B 66A-66C 1A-1A-3A 1A-1A-3C 1A-1A-5A 1A-1A-28A 1A-3A 1A-3A-3A 1A-3C 1A-5A 1A-7A 1A-7A-7A 1A-7C 1A-8A 1A-11A 1A-18A 1A-19A 1A-20A 1A-21A 1A-26A 1A-28A 1A-32A 1A-38A 1A-40A 1A-40C 1A-41A 1A-41C 1A-42A 1A-42C 1A-46A 1A-46C 1A-46D 1A-46E 1C-3A 1C-3C 1C-41A 2A-2A-4A 2A-2A-4A-4A 2A-2A-5A 2A-2A-12A 2A-2A-12B 2A-2A-13A 2A-2A-14A 2A-2A-29A 2A-2A-30A 2A-2A-46A 2A-2A-46C 2A-2A-46D 2A-2A-66A 2A-2A-66A-66A 2A-2A-66A-66B 2A-2A-66A-66C 2A-2A-66B 2A-2A-66C 2A-2A-71A 2A-4A 2A-4A-4A 2A-5A 2A-5B 2A-7A 2A-7A-7A 2A-12A 2A-12B 2A-13A 2A-14A 2A-17A 2A-28A 2A-29A 2A-30A 2A-46A 2A-46A-46A 2A-46A-46C 2A-46A-46D 2A-46C 2A-46D 2A-46E 2A-48A 2A-48A-48A 2A-48C 2A-49A 2A-66A 2A-66A-66A 2A-66A-66B 2A-66A-66C 2A-66B 2A-66C 2A-66D 2A-71A 2C-5A 2C-12A 2C-29A 2C-30A 2C-66A 2C-66A-66A 3A-3A-5A 3A-3A-7A 3A-3A-7A-7A 3A-3A-7C 3A-3A-8A 3A-3A-28A 3A-5A 3A-7A 3A-7A-7A 3A-7B 3A-7C 3A-7D 3A-8A 3A-11A 3A-19A 3A-20A 3A-21A 3A-26A 3A-28A 3A-32A 3A-38A 3A-40A 3A-40A-40A 3A-40C 3A-40D 3A-40E 3A-41A 3A-41C 3A-41D 3A-42A 3A-42C 3A-46A 3A-46A-46A 3A-46A-46C 3A-46C 3A-46D 3A-46E 3C-5A 3C-7A 3C-7C 3C-7D 3C-8A 3C-20A 3C-28A 3C-32A 3C-38A 3C-40A 3C-40C 3C-41A 3C-41C 3C-41D 4A-4A-5A 4A-4A-7A 4A-4A-12A 4A-4A-12B 4A-4A-13A 4A-4A-29A 4A-4A-30A 4A-4A-71A 4A-5A 4A-7A 4A-7A-7A 4A-7C 4A-12A 4A-12B 4A-13A 4A-17A 4A-28A 4A-29A 4A-30A 4A-46A 4A-46A-46A 4A-46A-46C 4A-46A-46D 4A-46C 4A-46D 4A-46E 4A-71A 5A-7A 5A-7A-7A 5A-7C 5A-25A 5A-30A 5A-38A 5A-40A 5A-41A 5A-46A 5A-46C 5A-46D 5A-46E 5A-48A 5A-66A 5A-66A-66A 5A-66A-66B 5A-66A-66C 5A-66B 5A-66C 5A-66D 5B-30A 5B-66A 5B-66A-66A 7A-7A-8A 7A-8A 7A-12A 7A-20A 7A-28A 7A-32A 7A-40A 7A-40C 7A-40D 7A-42A 7A-42C 7A-46A 7A-46C 7A-46D 7A-46E 7A-66A 7B-28A 7C-8A 7C-20A 7C-28A 7C-46D 8A-11A 8A-32A 8A-38A 8A-39A 8A-39C 8A-40A 8A-40A-40A 8A-41A 8A-41C 8A-41D 8A-42A 8A-42C 8A-46A 11A-18A 11A-28A 11A-41A 11A-41C 11A-42A 11A-42C 11A-46A 11A-46C 11A-46D 11A-46E 12A-25A 12A-30A 12A-46A 12A-66A 12A-66A-66A 12A-66C 12B-30A 13A-46A 13A-46C 13A-46D 13A-46E 13A-48A 13A-48C 13A-66A 13A-66A-66A 13A-66A-66B 13A-66A-66C 13A-66B 13A-66C 13A-66D 14A-30A 14A-66A 14A-66A-66A 19A-21A 19A-42A 19A-42C 20A-32A 20A-38A 20A-42A 20A-46A 20A-46C 21A-28A 21A-42A 21A-42C 25A-26A 25A-25A-26A 25A-41A 25A-41C 25A-41D 26A-41A 26A-41C 26A-41D 28A-40A 28A-40C 28A-40D 28A-41A 28A-41C 28A-42A 28A-42C 29A-30A 29A-66A 29A-66A-66A 30A-66A 30A-66A-66A 32A-38A 39A-41A 39A-41C 39A-41D 39A-46A 39C-41A 39C-41C 39C-41D 40A-46A 40A-46C 40A-46D 41A-42A 41A-42C 41A-42D 41A-46A 41A-46C 41A-46D 41A-46E 41C-42A 41C-42C 46A-46A-66A 46A-46C-66A 46A-46D-66A 46A-66A 46A-66A-66A 46C-66A 46C-66A-66A 46D-66A 46D-66A-66A 46D-66C 46E-66A 48A-48A-66A 48A-48A-66A-66A 48A-48A-66B 48A-48A-66C 48A-48C-66A 48A-48C-66B 48A-48C-66C 48A-66A 48A-66A-66A 48A-66B 48A-66C 48C-66A 48C-66B 48C-66C 66A-66A-71A 66A-71A 66C-71A 1A-1A-3A-5A 1A-1A-3A-28A 1A-3A-3A-7A 1A-3A-3A-7A-7A 1A-3A-3A-28A 1A-3A-5A 1A-3A-7A 1A-3A-7A-7A 1A-3A-7C 1A-3A-8A 1A-3A-11A 1A-3A-19A 1A-3A-20A 1A-3A-21A 1A-3A-26A 1A-3A-28A 1A-3A-32A 1A-3A-38A 1A-3A-40A 1A-3A-40C 1A-3A-41A 1A-3A-42A 1A-3A-42C 1A-3A-46A 1A-3A-46C 1A-3C-5A 1A-3C-7A 1A-3C-7C 1A-3C-8A 1A-3C-28A 1A-3C-41A 1A-5A-7A 1A-5A-7A-7A 1A-5A-40A 1A-7A-8A 1A-7A-20A 1A-7A-28A 1A-7A-32A 1A-7A-40A 1A-7A-40C 1A-7A-42A 1A-7A-46A 1A-7A-46C 1A-7A-46D 1A-7C-28A 1A-8A-11A 1A-8A-40A 1A-11A-18A 1A-19A-21A 1A-19A-42A 1A-19A-42C 1A-20A-32A 1A-20A-42A 1A-21A-28A 1A-21A-42A 1A-21A-42C 1A-28A-42A 1A-28A-42C 1A-41A-42A 1A-41A-42C 1A-41C-42A 1A-41C-42C 1C-3A-41A 1C-3C-41A 2A-2A-4A-5A 2A-2A-4A-12A 2A-2A-4A-71A 2A-2A-5A-30A 2A-2A-5A-66A 2A-2A-5A-66A-66A 2A-2A-5A-66B 2A-2A-5A-66C 2A-2A-12A-30A 2A-2A-12A-66A 2A-2A-12A-66A-66A 2A-2A-12A-66C 2A-2A-12B-30A 2A-2A-13A-66A 2A-2A-13A-66B 2A-2A-13A-66C 2A-2A-14A-30A 2A-2A-14A-66A 2A-2A-14A-66A-66A 2A-2A-29A-30A 2A-2A-29A-66A 2A-2A-29A-66A-66A 2A-2A-30A-66A 2A-2A-66A-71A 2A-4A-4A-5A 2A-4A-4A-12A 2A-4A-4A-71A 2A-4A-5A 2A-4A-7A 2A-4A-7A-7A 2A-4A-12A 2A-4A-12B 2A-4A-13A 2A-4A-29A 2A-4A-30A 2A-4A-71A 2A-5A-30A 2A-5A-46A 2A-5A-46C 2A-5A-46D 2A-5A-66A 2A-5A-66A-66A 2A-5A-66B 2A-5A-66C 2A-5B-30A 2A-5B-66A 2A-5B-66A-66A 2A-7A-12A 2A-7A-66A 2A-12A-30A 2A-12A-46A 2A-12A-66A 2A-12A-66A-66A 2A-12A-66C 2A-12B-30A 2A-13A-46A 2A-13A-46C 2A-13A-46D 2A-13A-66A 2A-13A-66A-66A 2A-13A-66B 2A-13A-66C 2A-14A-30A 2A-14A-66A 2A-14A-66A-66A 2A-29A-30A 2A-29A-66A 2A-29A-66A-66A 2A-30A-66A 2A-30A-66A-66A 2A-46A-46A-66A 2A-46A-46C-66A 2A-46A-66A 2A-46C-66A 2A-46D-66A 2A-48A-66A 2A-66A-71A 2A-66A-66A-71A 2A-66C-71A 2C-12A-66A 2C-29A-66A 3A-3A-5A-7A 3A-3A-7A-7A-8A 3A-3A-7A-8A 3A-3A-7A-28A 3A-5A-7A 3A-5A-7A-7A 3A-5A-7C 3A-5A-38A 3A-5A-40A 3A-7A-7A-8A 3A-7A-8A 3A-7A-20A 3A-7A-28A 3A-7A-32A 3A-7A-38A 3A-7A-40A 3A-7A-40C 3A-7A-42A 3A-7A-46A 3A-7A-46C 3A-7A-46D 3A-7C-8A 3A-7C-20A 3A-7C-28A 3A-8A-11A 3A-8A-32A 3A-8A-38A 3A-8A-40A 3A-8A-46A 3A-19A-21A 3A-19A-42A 3A-19A-42C 3A-20A-32A 3A-20A-38A 3A-20A-42A 3A-20A-46A 3A-21A-28A 3A-21A-42A 3A-21A-42C 3A-28A-40A 3A-28A-40C 3A-28A-40D 3A-28A-41A 3A-28A-41C 3A-28A-42A 3A-28A-42C 3A-32A-38A 3A-41A-42A 3A-41A-42C 3A-41C-42A 3A-41C-42C 3C-7A-20A 3C-7A-28A 3C-7A-32A 3C-7A-38A 3C-7C-28A 3C-20A-32A 3C-20A-38A 4A-4A-5A-30A 4A-4A-12A-30A 4A-5A-30A 4A-7A-12A 4A-12A-30A 4A-12A-46A 4A-29A-30A 5A-30A-66A 5A-30A-66A-66A 5A-46A-66A 5A-46C-66A 5A-46D-66A 5B-30A-66A 5B-30A-66A-66A 7A-8A-46A 7A-20A-32A 7A-20A-38A 7A-20A-42A 7A-20A-46A 7A-20A-46C 12A-30A-66A 12A-30A-66A-66A 13A-46A-66A 13A-46C-66A 13A-46D-66A 13A-48A-66A 13A-48A-66B 13A-48A-66C 13A-48C-66A 13A-48C-66B 13A-48C-66C 14A-30A-66A 14A-30A-66A-66A 19A-21A-42A 19A-21A-42C 20A-32A-38A 21A-28A-42A 21A-28A-42C 28A-41A-42A 28A-41A-42C 28A-41C-42A 28A-41C-42C 29A-30A-66A 29A-30A-66A-66A 1A-3A-5A-7A 1A-3A-5A-7A-7A 1A-3A-5A-40A 1A-3A-7A-8A 1A-3A-7A-20A 1A-3A-7A-28A 1A-3A-7A-40A 1A-3A-7A-40C 1A-3A-7A-42A 1A-3A-7C-28A 1A-3A-8A-11A 1A-3A-19A-21A 1A-3A-19A-42A 1A-3A-19A-42C 1A-3A-20A-32A 1A-3A-20A-42A 1A-3A-21A-28A 1A-3A-21A-42A 1A-3A-21A-42C 1A-3A-28A-42A 1A-3A-28A-42C 1A-7A-20A-32A 1A-7A-20A-42A 1A-19A-21A-42A 1A-19A-21A-42C 1A-21A-28A-42A 1A-21A-28A-42C 2A-2A-5A-30A-66A 2A-2A-12A-30A-66A 2A-2A-14A-30A-66A 2A-2A-29A-30A-66A 2A-4A-5A-30A 2A-4A-7A-12A 2A-4A-12A-30A 2A-4A-29A-30A 2A-5A-30A-66A 2A-5A-30A-66A-66A 2A-5B-30A-66A 2A-12A-30A-66A 2A-12A-30A-66A-66A 2A-14A-30A-66A 2A-14A-30A-66A-66A 2A-29A-30A-66A 2A-29A-30A-66A-66A 3A-7A-20A-32A 3A-7A-20A-38A 3A-7A-20A-42A 3A-7A-32A-38A 3A-20A-32A-38A 3A-28A-41A-42A 3A-28A-41A-42C 3A-28A-41C-42A 3C-7A-20A-32A 3C-7A-20A-38A 7A-20A-32A-38A 1A-3A-7A-20A-42A 3A-7A-20A-32A-38A I II IV V VI VIII 850 900 1800 1900 A E F', '3C 7C 8B 38C 39C 40C 40D 41C 41D 42C 1A-5A 1A-8A 1A-18A 1A-19A 1A-20A 1A-28A 2A-5A 2A-12A 2A-13A 3A-8A 3A-5A 3A-19A 3A-20A 3A-26A 3A-28A 4A-5A 4A-12A 4A-13A 4A-17A 5A-7A 5A-30A 5A-40A 5A-66A 7A-8A 7A-20A 7A-28A 8A-39A 8A-41A 12A-30A 12A-66A 13A-66A 19A-42A 28A-42A', '1 2 3 4');
/*!40000 ALTER TABLE `iceband` ENABLE KEYS */;

-- Dumping structure for procedure ctpstestplandb.insertdata
DROP PROCEDURE IF EXISTS `insertdata`;
DELIMITER //
CREATE DEFINER=`su`@`%` PROCEDURE `insertdata`(
	IN `st` VARCHAR(50),
	IN `tcno` VARCHAR(50),
	IN `ba` VARCHAR(50),
	IN `des` VARCHAR(550),
	IN `gcatp` VARCHAR(200),
	IN `gcsdtp` VARCHAR(200),
	IN `tcstat` VARCHAR(200),
	IN `picsstat` VARCHAR(550),
	IN `envcon` VARCHAR(200),
	IN `bs` VARCHAR(200),
	IN `icebs` VARCHAR(200),
	IN `ver` VARCHAR(50),
	IN `picsver` VARCHAR(50),
	IN `sh` VARCHAR(100)
)
    NO SQL
BEGIN
DECLARE id_tcconfig int;
DECLARE id_tcdata int;
INSERT INTO tcconfigtable (Standards,TCNumber,Description,Band,SheetName)
                            SELECT st,tcno,des,ba,sh
                            FROM dual
                            WHERE NOT EXISTS(SELECT * FROM tcconfigtable WHERE Band = ba and TCNumber = tcno and Standards = st);
select id into id_tcconfig from tcconfigtable where Band = ba and TCNumber = tcno and Standards = st;
INSERT INTO tcdatatable (`F_ID`,`GCF_Certified_All_TP`,`GCF_Certified_SD_TP`,`TC_Status`,`PICS_Status`,`Env_Cond`,`Band_Support`,`ICE_Band_Support`)
                            SELECT id_tcconfig,gcatp,gcsdtp,tcstat,picsstat,envcon,bs,icebs
                            FROM dual
                            WHERE NOT EXISTS(SELECT * FROM tcdatatable WHERE `F_ID`=id_tcconfig and `GCF_Certified_All_TP`= gcatp and`GCF_Certified_SD_TP`=gcsdtp and `TC_Status`= tcstat and `PICS_Status`=picsstat and `Env_Cond`= envcon and `Band_Support`=bs and `ICE_Band_Support` =icebs);
select id into id_tcdata from tcdatatable where `F_ID`=id_tcconfig and `GCF_Certified_All_TP`= gcatp and`GCF_Certified_SD_TP`=gcsdtp and `TC_Status`= tcstat and `PICS_Status`=picsstat and `Env_Cond`= envcon and `Band_Support`=bs and `ICE_Band_Support` =icebs;
INSERT INTO mapping (`DATAID`,`VER`,`PICS_Ver`) select id_tcdata, ver,picsver ;
END//
DELIMITER ;

-- Dumping structure for table ctpstestplandb.mapping_tc_band_table
DROP TABLE IF EXISTS `mapping_tc_band_table`;
CREATE TABLE IF NOT EXISTS `mapping_tc_band_table` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `id#tc` int(11) DEFAULT NULL,
  `id#testbandconfig` int(11) DEFAULT NULL,
  `id#tcdata` int(11) DEFAULT NULL,
  `id#ver` int(11) DEFAULT NULL,
  `tcstatus` varchar(2) DEFAULT '',
  `wi_rft` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id#tc_id#testbandconfig_id#tcdata_id#ver` (`id#tc`,`id#testbandconfig`,`id#ver`)
) ENGINE=InnoDB AUTO_INCREMENT=816645 DEFAULT CHARSET=utf8;

-- Dumping data for table ctpstestplandb.mapping_tc_band_table: ~47,040 rows (approximately)
/*!40000 ALTER TABLE `mapping_tc_band_table` DISABLE KEYS */;
/*!40000 ALTER TABLE `mapping_tc_band_table` ENABLE KEYS */;

-- Dumping structure for table ctpstestplandb.tcdata
DROP TABLE IF EXISTS `tcdata`;
CREATE TABLE IF NOT EXISTS `tcdata` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `certified_tp_v` varchar(500) DEFAULT '',
  `certified_tp_e` varchar(100) DEFAULT '',
  `certified_tp_d` varchar(100) DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `certified_tp_v_certified_tp_e_certified_tp_d` (`certified_tp_v`,`certified_tp_e`,`certified_tp_d`)
) ENGINE=InnoDB AUTO_INCREMENT=23405 DEFAULT CHARSET=utf8;

-- Dumping data for table ctpstestplandb.tcdata: ~1,020 rows (approximately)
/*!40000 ALTER TABLE `tcdata` DISABLE KEYS */;
/*!40000 ALTER TABLE `tcdata` ENABLE KEYS */;

-- Dumping structure for table ctpstestplandb.testbandconfig
DROP TABLE IF EXISTS `testbandconfig`;
CREATE TABLE IF NOT EXISTS `testbandconfig` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `band` varchar(50) DEFAULT '',
  `band2` varchar(50) DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `band` (`band`)
) ENGINE=InnoDB AUTO_INCREMENT=13466 DEFAULT CHARSET=utf8;

-- Dumping data for table ctpstestplandb.testbandconfig: ~437 rows (approximately)
/*!40000 ALTER TABLE `testbandconfig` DISABLE KEYS */;
/*!40000 ALTER TABLE `testbandconfig` ENABLE KEYS */;

-- Dumping structure for table ctpstestplandb.testcasetable
DROP TABLE IF EXISTS `testcasetable`;
CREATE TABLE IF NOT EXISTS `testcasetable` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `spec` varchar(50) NOT NULL DEFAULT '',
  `testcase` varchar(50) NOT NULL DEFAULT '',
  `description` varchar(550) DEFAULT '',
  `id#envcond` int(2) NOT NULL,
  `id#band_app_cri` int(3) NOT NULL,
  `sheetname` varchar(100) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_spec_testcase_sheetname` (`spec`,`testcase`,`sheetname`),
  KEY `FK_env` (`id#envcond`),
  KEY `FK_band` (`id#band_app_cri`),
  CONSTRAINT `FK_band` FOREIGN KEY (`id#band_app_cri`) REFERENCES `band_app_cri_table` (`id`),
  CONSTRAINT `FK_env` FOREIGN KEY (`id#envcond`) REFERENCES `envcond` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=110006 DEFAULT CHARSET=utf8;

-- Dumping data for table ctpstestplandb.testcasetable: ~4,036 rows (approximately)
/*!40000 ALTER TABLE `testcasetable` DISABLE KEYS */;
/*!40000 ALTER TABLE `testcasetable` ENABLE KEYS */;

-- Dumping structure for table ctpstestplandb.user_bs_rb_pics
DROP TABLE IF EXISTS `user_bs_rb_pics`;
CREATE TABLE IF NOT EXISTS `user_bs_rb_pics` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `bandsupport` varchar(5) NOT NULL DEFAULT '',
  `requiredband` varchar(5) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8;

-- Dumping data for table ctpstestplandb.user_bs_rb_pics: ~11 rows (approximately)
/*!40000 ALTER TABLE `user_bs_rb_pics` DISABLE KEYS */;
REPLACE INTO `user_bs_rb_pics` (`id`, `bandsupport`, `requiredband`) VALUES
	(1, 'S', 'Y'),
	(2, 'S', 'N'),
	(3, 'NS', 'Y'),
	(4, 'NS', 'N'),
	(5, 'NA', 'Y'),
	(6, 'NA', 'N'),
	(7, '', 'Y'),
	(8, '', 'N'),
	(9, 'S', ''),
	(10, 'NS', ''),
	(11, '', '');
/*!40000 ALTER TABLE `user_bs_rb_pics` ENABLE KEYS */;

-- Dumping structure for table ctpstestplandb.user_picsmappingtable
DROP TABLE IF EXISTS `user_picsmappingtable`;
CREATE TABLE IF NOT EXISTS `user_picsmappingtable` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `id#pics` int(11) NOT NULL,
  `id#bsrb` int(11) NOT NULL,
  `Id#picsstat` int(11) NOT NULL,
  `Id#v_comb_serv_info` int(11) NOT NULL,
  `icebs` varchar(10) DEFAULT '',
  `PICSLogic` text,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id#pics_id#bsrb_Id#picsstat_Id#v_comb_serv_info` (`id#pics`,`id#bsrb`,`Id#picsstat`,`Id#v_comb_serv_info`)
) ENGINE=InnoDB AUTO_INCREMENT=4725759 DEFAULT CHARSET=utf8;

-- Dumping data for table ctpstestplandb.user_picsmappingtable: ~47,014 rows (approximately)
/*!40000 ALTER TABLE `user_picsmappingtable` DISABLE KEYS */;
/*!40000 ALTER TABLE `user_picsmappingtable` ENABLE KEYS */;

-- Dumping structure for table ctpstestplandb.user_picsstatus
DROP TABLE IF EXISTS `user_picsstatus`;
CREATE TABLE IF NOT EXISTS `user_picsstatus` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `picsstatus` varchar(500) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `picsstatus` (`picsstatus`)
) ENGINE=InnoDB AUTO_INCREMENT=3628 DEFAULT CHARSET=utf8;

-- Dumping data for table ctpstestplandb.user_picsstatus: ~55 rows (approximately)
/*!40000 ALTER TABLE `user_picsstatus` DISABLE KEYS */;
/*!40000 ALTER TABLE `user_picsstatus` ENABLE KEYS */;

-- Dumping structure for table ctpstestplandb.user_picsver
DROP TABLE IF EXISTS `user_picsver`;
CREATE TABLE IF NOT EXISTS `user_picsver` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `picsver` varchar(50) NOT NULL DEFAULT '',
  `picssupportedbandlist` varchar(1500) NOT NULL DEFAULT '',
  `id#gcfver` int(11) NOT NULL,
  `gcfver` varchar(100) DEFAULT NULL,
  `icebands` text,
  `icebands_ulca` text,
  `4x4_mimo` text,
  PRIMARY KEY (`id`),
  UNIQUE KEY `picsver_id#gcfver` (`picsver`,`id#gcfver`)
) ENGINE=InnoDB AUTO_INCREMENT=218 DEFAULT CHARSET=utf8;

-- Dumping data for table ctpstestplandb.user_picsver: ~2 rows (approximately)
/*!40000 ALTER TABLE `user_picsver` DISABLE KEYS */;
/*!40000 ALTER TABLE `user_picsver` ENABLE KEYS */;

-- Dumping structure for view ctpstestplandb.v
DROP VIEW IF EXISTS `v`;
-- Creating temporary table to overcome VIEW dependency errors
CREATE TABLE `v` (
	`id#tcconf` INT(11) NULL,
	`id#bandconfig` INT(11) NULL,
	`id#bsrb` INT(11) NULL,
	`id#user_picsstatus` INT(11) NULL,
	`id_ver` INT(11) NULL,
	`id#tcdata` INT(11) NULL,
	`id#picsmapping` INT(11) NOT NULL,
	`GCF/PTCRB/Operator Version` VARCHAR(100) NULL COLLATE 'utf8_general_ci',
	`PICS Version` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`Spec` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`SheetName` VARCHAR(100) NULL COLLATE 'utf8_general_ci',
	`Test Case Number` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`Description` VARCHAR(550) NULL COLLATE 'utf8_general_ci',
	`Band_old` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`Band` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`Band Applicability` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`Band Criteria` VARCHAR(10) NULL COLLATE 'utf8_general_ci',
	`Cert TP [V]` VARCHAR(500) NULL COLLATE 'utf8_general_ci',
	`Cert TP [E]` VARCHAR(100) NULL COLLATE 'utf8_general_ci',
	`Cert TP [D]` VARCHAR(100) NULL COLLATE 'utf8_general_ci',
	`TC Status` VARCHAR(2) NULL COLLATE 'utf8_general_ci',
	`PICS Status` VARCHAR(500) NULL COLLATE 'utf8_general_ci',
	`Env_Cond` VARCHAR(40) NULL COLLATE 'utf8_general_ci',
	`Band Support` VARCHAR(5) NULL COLLATE 'utf8_general_ci',
	`ICE Band Support` VARCHAR(10) NULL COLLATE 'utf8_general_ci',
	`Required Bands` VARCHAR(5) NULL COLLATE 'utf8_general_ci',
	`PICSLogic` TEXT NULL COLLATE 'utf8_general_ci',
	`wi_rft` VARCHAR(100) NULL COLLATE 'utf8_general_ci'
) ENGINE=MyISAM;

-- Dumping structure for view ctpstestplandb.v_comb_serv_info
DROP VIEW IF EXISTS `v_comb_serv_info`;
-- Creating temporary table to overcome VIEW dependency errors
CREATE TABLE `v_comb_serv_info` (
	`id` INT(11) NOT NULL,
	`id_tc` INT(11) NULL,
	`id_band` INT(11) NULL,
	`id_data` INT(11) NULL,
	`id_gcfver` INT(11) NULL,
	`spec` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`testcase` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`description` VARCHAR(550) NULL COLLATE 'utf8_general_ci',
	`sheetname` VARCHAR(100) NULL COLLATE 'utf8_general_ci',
	`envcond` VARCHAR(40) NULL COLLATE 'utf8_general_ci',
	`bandapplicability` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`bandcriteria` VARCHAR(10) NULL COLLATE 'utf8_general_ci',
	`band` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`band2` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`tcstatus` VARCHAR(2) NULL COLLATE 'utf8_general_ci',
	`icebandsupport` CHAR(0) NOT NULL COLLATE 'utf8mb4_general_ci',
	`certified_tp_v` VARCHAR(500) NULL COLLATE 'utf8_general_ci',
	`certified_tp_e` VARCHAR(100) NULL COLLATE 'utf8_general_ci',
	`certified_tp_d` VARCHAR(100) NULL COLLATE 'utf8_general_ci',
	`ver_gcf_ptcrb_op` VARCHAR(100) NULL COLLATE 'utf8_general_ci',
	`wi_rft` VARCHAR(100) NULL COLLATE 'utf8_general_ci'
) ENGINE=MyISAM;

-- Dumping structure for view ctpstestplandb.v_testcase
DROP VIEW IF EXISTS `v_testcase`;
-- Creating temporary table to overcome VIEW dependency errors
CREATE TABLE `v_testcase` (
	`id` INT(11) NOT NULL,
	`spec` VARCHAR(50) NOT NULL COLLATE 'utf8_general_ci',
	`testcase` VARCHAR(50) NOT NULL COLLATE 'utf8_general_ci',
	`description` VARCHAR(550) NULL COLLATE 'utf8_general_ci',
	`sheetname` VARCHAR(100) NOT NULL COLLATE 'utf8_general_ci',
	`envcond` VARCHAR(40) NULL COLLATE 'utf8_general_ci',
	`bandapplicability` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`bandcriteria` VARCHAR(10) NULL COLLATE 'utf8_general_ci'
) ENGINE=MyISAM;

-- Dumping structure for view ctpstestplandb.v_userpics
DROP VIEW IF EXISTS `v_userpics`;
-- Creating temporary table to overcome VIEW dependency errors
CREATE TABLE `v_userpics` (
	`id#bsrb` INT(11) NULL,
	`id_ver` INT(11) NULL,
	`id` INT(11) NOT NULL,
	`Id#v_comb_serv_info` INT(11) NOT NULL,
	`Ice Band Support` VARCHAR(10) NULL COLLATE 'utf8_general_ci',
	`PICSLogic` TEXT NULL COLLATE 'utf8_general_ci',
	`bandsupport` VARCHAR(5) NULL COLLATE 'utf8_general_ci',
	`requiredband` VARCHAR(5) NULL COLLATE 'utf8_general_ci',
	`id#user_picsstatus` INT(11) NULL,
	`picsstatus` VARCHAR(500) NULL COLLATE 'utf8_general_ci',
	`id#user_picsver` INT(11) NULL,
	`picsver` VARCHAR(50) NULL COLLATE 'utf8_general_ci',
	`Picssupportedbandlist` VARCHAR(1500) NULL COLLATE 'utf8_general_ci',
	`ver_gcf_ptcrb_op` VARCHAR(100) NULL COLLATE 'utf8_general_ci'
) ENGINE=MyISAM;

-- Dumping structure for trigger ctpstestplandb.addiceband
DROP TRIGGER IF EXISTS `addiceband`;
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `addiceband` BEFORE INSERT ON `user_picsver` FOR EACH ROW BEGIN
set new.icebands =
(

SELECT  bandlist
FROM
   ( 
SELECT @row_num := IF(@prev_value=ib.iceproj,@row_num+1,1) AS rn
		,ib.id
      ,ib.bandlist as bandlist
		,ib.iceproj
      ,@prev_value := ib.iceproj as pr
FROM iceband ib,
      (SELECT @row_num := 1) x,
      (SELECT @prev_value := '') y
--      (select @prj :='%74_0%') z
where   new.picsver like concat('%',ib.iceproj ,'%')
ORDER BY  ib.iceproj,ib.id DESC )a
WHERE rn = 1 

);
set new.icebands_ulca =
(

SELECT  bandlist
FROM
   ( 
SELECT @row_num := IF(@prev_value=ib.iceproj,@row_num+1,1) AS rn
		,ib.id
      ,ib.bandlist_ulca as bandlist
		,ib.iceproj
      ,@prev_value := ib.iceproj as pr
FROM iceband ib,
      (SELECT @row_num := 1) x,
      (SELECT @prev_value := '') y
--      (select @prj :='%74_0%') z
where   new.picsver like concat('%',ib.iceproj ,'%')
ORDER BY  ib.iceproj,ib.id DESC )a
WHERE rn = 1 

);

set new.4x4_mimo =
(

SELECT  bandlist
FROM
   ( 
SELECT @row_num := IF(@prev_value=ib.iceproj,@row_num+1,1) AS rn
		,ib.id
      ,ib.4x4_mimo as bandlist
		,ib.iceproj
      ,@prev_value := ib.iceproj as pr
FROM iceband ib,
      (SELECT @row_num := 1) x,
      (SELECT @prev_value := '') y
--      (select @prj :='%74_0%') z
where   new.picsver like concat('%',ib.iceproj ,'%')
ORDER BY  ib.iceproj,ib.id DESC )a
WHERE rn = 1 

);

END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Dumping structure for view ctpstestplandb.v
DROP VIEW IF EXISTS `v`;
-- Removing temporary table and create final VIEW structure
DROP TABLE IF EXISTS `v`;
CREATE ALGORITHM=UNDEFINED DEFINER=`su`@`%` SQL SECURITY DEFINER VIEW `v` AS (select `vcsi`.`id_tc` AS `id#tcconf`,`vcsi`.`id_band` AS `id#bandconfig`,`vup`.`id#bsrb` AS `id#bsrb`,`vup`.`id#user_picsstatus` AS `id#user_picsstatus`,`vup`.`id_ver` AS `id_ver`,`vcsi`.`id_data` AS `id#tcdata`,`vup`.`id` AS `id#picsmapping`,`vup`.`ver_gcf_ptcrb_op` AS `GCF/PTCRB/Operator Version`,`vup`.`picsver` AS `PICS Version`,`vcsi`.`spec` AS `Spec`,`vcsi`.`sheetname` AS `SheetName`,`vcsi`.`testcase` AS `Test Case Number`,`vcsi`.`description` AS `Description`,`vcsi`.`band` AS `Band_old`,`vcsi`.`band2` AS `Band`,`vcsi`.`bandapplicability` AS `Band Applicability`,`vcsi`.`bandcriteria` AS `Band Criteria`,`vcsi`.`certified_tp_v` AS `Cert TP [V]`,`vcsi`.`certified_tp_e` AS `Cert TP [E]`,`vcsi`.`certified_tp_d` AS `Cert TP [D]`,`vcsi`.`tcstatus` AS `TC Status`,`vup`.`picsstatus` AS `PICS Status`,`vcsi`.`envcond` AS `Env_Cond`,`vup`.`bandsupport` AS `Band Support`,`vup`.`Ice Band Support` AS `ICE Band Support`,`vup`.`requiredband` AS `Required Bands`,`vup`.`PICSLogic` AS `PICSLogic`,`vcsi`.`wi_rft` AS `wi_rft` from (`v_userpics` `vup` left join `v_comb_serv_info` `vcsi` on((`vup`.`Id#v_comb_serv_info` = `vcsi`.`id`))));

-- Dumping structure for view ctpstestplandb.v_comb_serv_info
DROP VIEW IF EXISTS `v_comb_serv_info`;
-- Removing temporary table and create final VIEW structure
DROP TABLE IF EXISTS `v_comb_serv_info`;
CREATE ALGORITHM=UNDEFINED DEFINER=`su`@`%` SQL SECURITY DEFINER VIEW `v_comb_serv_info` AS select `m`.`id` AS `id`,`m`.`id#tc` AS `id_tc`,`m`.`id#testbandconfig` AS `id_band`,`m`.`id#tcdata` AS `id_data`,`m`.`id#ver` AS `id_gcfver`,`t`.`spec` AS `spec`,`t`.`testcase` AS `testcase`,`t`.`description` AS `description`,`t`.`sheetname` AS `sheetname`,`t`.`envcond` AS `envcond`,`t`.`bandapplicability` AS `bandapplicability`,`t`.`bandcriteria` AS `bandcriteria`,`b`.`band` AS `band`,`b`.`band2` AS `band2`,`m`.`tcstatus` AS `tcstatus`,'' AS `icebandsupport`,`d`.`certified_tp_v` AS `certified_tp_v`,`d`.`certified_tp_e` AS `certified_tp_e`,`d`.`certified_tp_d` AS `certified_tp_d`,`ver`.`ver_gcf_ptcrb_op` AS `ver_gcf_ptcrb_op`,`m`.`wi_rft` AS `wi_rft` from ((((`mapping_tc_band_table` `m` left join `v_testcase` `t` on((`m`.`id#tc` = `t`.`id`))) left join `testbandconfig` `b` on((`m`.`id#testbandconfig` = `b`.`id`))) left join `tcdata` `d` on((`m`.`id#tcdata` = `d`.`id`))) left join `gcfptcrbver` `ver` on((`m`.`id#ver` = `ver`.`id`)));

-- Dumping structure for view ctpstestplandb.v_testcase
DROP VIEW IF EXISTS `v_testcase`;
-- Removing temporary table and create final VIEW structure
DROP TABLE IF EXISTS `v_testcase`;
CREATE ALGORITHM=UNDEFINED DEFINER=`su`@`%` SQL SECURITY DEFINER VIEW `v_testcase` AS select `tc`.`id` AS `id`,`tc`.`spec` AS `spec`,`tc`.`testcase` AS `testcase`,`tc`.`description` AS `description`,`tc`.`sheetname` AS `sheetname`,`env`.`envcond` AS `envcond`,`bac`.`bandapplicability` AS `bandapplicability`,`bac`.`bandcriteria` AS `bandcriteria` from ((`testcasetable` `tc` left join `envcond` `env` on((`tc`.`id#envcond` = `env`.`id`))) left join `band_app_cri_table` `bac` on((`tc`.`id#band_app_cri` = `bac`.`id`)));

-- Dumping structure for view ctpstestplandb.v_userpics
DROP VIEW IF EXISTS `v_userpics`;
-- Removing temporary table and create final VIEW structure
DROP TABLE IF EXISTS `v_userpics`;
CREATE ALGORITHM=UNDEFINED DEFINER=`su`@`%` SQL SECURITY DEFINER VIEW `v_userpics` AS select `b`.`id` AS `id#bsrb`,`pv`.`id` AS `id_ver`,`m`.`id` AS `id`,`m`.`Id#v_comb_serv_info` AS `Id#v_comb_serv_info`,`m`.`icebs` AS `Ice Band Support`,`m`.`PICSLogic` AS `PICSLogic`,`b`.`bandsupport` AS `bandsupport`,`b`.`requiredband` AS `requiredband`,`ps`.`id` AS `id#user_picsstatus`,`ps`.`picsstatus` AS `picsstatus`,`pv`.`id` AS `id#user_picsver`,`pv`.`picsver` AS `picsver`,`pv`.`picssupportedbandlist` AS `Picssupportedbandlist`,`gpv`.`ver_gcf_ptcrb_op` AS `ver_gcf_ptcrb_op` from ((((`user_picsmappingtable` `m` left join `user_bs_rb_pics` `b` on((`m`.`id#bsrb` = `b`.`id`))) left join `user_picsstatus` `ps` on((`m`.`Id#picsstat` = `ps`.`id`))) left join `user_picsver` `pv` on((`m`.`id#pics` = `pv`.`id`))) left join `gcfptcrbver` `gpv` on((`pv`.`id#gcfver` = `gpv`.`id`)));

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

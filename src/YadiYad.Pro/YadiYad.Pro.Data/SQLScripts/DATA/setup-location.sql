
DROP TEMPORARY TABLE IF EXISTS `tmpLocation`;

CREATE TEMPORARY TABLE `tmpLocation`(
  `Country` varchar(200) NOT NULL,
  `State` varchar(200) NOT NULL,
  `City` varchar(200) NOT NULL
);

INSERT INTO `tmpLocation` 
(`Country`, `State`, `City`) VALUES 
('Malaysia', 'Johor', 'Johor Bahru'),
('Malaysia', 'Johor', 'Tebrau'),
('Malaysia', 'Johor', 'Pasir Gudang'),
('Malaysia', 'Johor', 'Bukit Indah'),
('Malaysia', 'Johor', 'Skudai'),
('Malaysia', 'Johor', 'Kluang'),
('Malaysia', 'Johor', 'Batu Pahat'),
('Malaysia', 'Johor', 'Muar'),
('Malaysia', 'Johor', 'Ulu Tiram'),
('Malaysia', 'Johor', 'Senai'),
('Malaysia', 'Johor', 'Segamat'),
('Malaysia', 'Johor', 'Kulai'),
('Malaysia', 'Johor', 'Kota Tinggi'),
('Malaysia', 'Johor', 'Pontian Kechil'),
('Malaysia', 'Johor', 'Tangkak'),
('Malaysia', 'Johor', 'Bukit Bakri'),
('Malaysia', 'Johor', 'Yong Peng'),
('Malaysia', 'Johor', 'Pekan Nenas'),
('Malaysia', 'Johor', 'Labis'),
('Malaysia', 'Johor', 'Mersing'),
('Malaysia', 'Johor', 'Simpang Renggam'),
('Malaysia', 'Johor', 'Parit Raja'),
('Malaysia', 'Johor', 'Kelapa Sawit'),
('Malaysia', 'Johor', 'Buloh Kasap'),
('Malaysia', 'Johor', 'Chaah'),
('Malaysia', 'Kedah', 'Sungai Petani'),
('Malaysia', 'Kedah', 'Alor Setar'),
('Malaysia', 'Kedah', 'Kulim'),
('Malaysia', 'Kedah', 'Jitra / Kubang Pasu'),
('Malaysia', 'Kedah', 'Baling'),
('Malaysia', 'Kedah', 'Pendang'),
('Malaysia', 'Kedah', 'Langkawi'),
('Malaysia', 'Kedah', 'Yan'),
('Malaysia', 'Kedah', 'Sik'),
('Malaysia', 'Kedah', 'Kuala Nerang'),
('Malaysia', 'Kedah', 'Pokok Sena'),
('Malaysia', 'Kedah', 'Bandar Baharu'),
('Malaysia', 'Kelantan', 'Kota Bharu'),
('Malaysia', 'Kelantan', 'Pangkal Kalong'),
('Malaysia', 'Kelantan', 'Tanah Merah'),
('Malaysia', 'Kelantan', 'Peringat'),
('Malaysia', 'Kelantan', 'Wakaf Baru'),
('Malaysia', 'Kelantan', 'Kadok'),
('Malaysia', 'Kelantan', 'Pasir Mas'),
('Malaysia', 'Kelantan', 'Gua Musang'),
('Malaysia', 'Kelantan', 'Kuala Krai'),
('Malaysia', 'Kelantan', 'Tumpat'),
('Malaysia', 'Melaka', 'Bandaraya Melaka'),
('Malaysia', 'Melaka', 'Bukit Baru'),
('Malaysia', 'Melaka', 'Ayer Keroh'),
('Malaysia', 'Melaka', 'Klebang'),
('Malaysia', 'Melaka', 'Masjid Tanah'),
('Malaysia', 'Melaka', 'Sungai Udang'),
('Malaysia', 'Melaka', 'Batu Berendam'),
('Malaysia', 'Melaka', 'Alor Gajah'),
('Malaysia', 'Melaka', 'Bukit Rambai'),
('Malaysia', 'Melaka', 'Ayer Molek'),
('Malaysia', 'Melaka', 'Bemban'),
('Malaysia', 'Melaka', 'Kuala Sungai Baru'),
('Malaysia', 'Melaka', 'Pulau Sebang'),
('Malaysia', 'Negeri Sembilan', 'Seremban'),
('Malaysia', 'Negeri Sembilan', 'Port Dickson'),
('Malaysia', 'Negeri Sembilan', 'Nilai'),
('Malaysia', 'Negeri Sembilan', 'Bahau'),
('Malaysia', 'Negeri Sembilan', 'Tampin'),
('Malaysia', 'Negeri Sembilan', 'Kuala Pilah'),
('Malaysia', 'Pahang', 'Kuantan'),
('Malaysia', 'Pahang', 'Temerloh'),
('Malaysia', 'Pahang', 'Bentong'),
('Malaysia', 'Pahang', 'Mentakab'),
('Malaysia', 'Pahang', 'Raub'),
('Malaysia', 'Pahang', 'Jerantut'),
('Malaysia', 'Pahang', 'Pekan'),
('Malaysia', 'Pahang', 'Kuala Lipis'),
('Malaysia', 'Pahang', 'Bandar Jengka'),
('Malaysia', 'Pahang', 'Bukit Tinggi'),
('Malaysia', 'Perak', 'Ipoh'),
('Malaysia', 'Perak', 'Taiping'),
('Malaysia', 'Perak', 'Sitiawan'),
('Malaysia', 'Perak', 'Simpang Empat'),
('Malaysia', 'Perak', 'Teluk Intan'),
('Malaysia', 'Perak', 'Batu Gajah'),
('Malaysia', 'Perak', 'Lumut'),
('Malaysia', 'Perak', 'Kampung Koh'),
('Malaysia', 'Perak', 'Kuala Kangsar'),
('Malaysia', 'Perak', 'Sungai Siput Utara'),
('Malaysia', 'Perak', 'Tapah'),
('Malaysia', 'Perak', 'Bidor'),
('Malaysia', 'Perak', 'Parit Buntar'),
('Malaysia', 'Perak', 'Ayer Tawar'),
('Malaysia', 'Perak', 'Bagan Serai'),
('Malaysia', 'Perak', 'Tanjung Malim'),
('Malaysia', 'Perak', 'Lawan Kuda Baharu'),
('Malaysia', 'Perak', 'Pantai Remis'),
('Malaysia', 'Perak', 'Kampar'),
('Malaysia', 'Perlis', 'Kangar'),
('Malaysia', 'Perlis', 'Kuala Perlis'),
('Malaysia', 'Pulau Pinang', 'Bukit Mertajam'),
('Malaysia', 'Pulau Pinang', 'Georgetown'),
('Malaysia', 'Pulau Pinang', 'Sungai Ara'),
('Malaysia', 'Pulau Pinang', 'Gelugor'),
('Malaysia', 'Pulau Pinang', 'Ayer Itam'),
('Malaysia', 'Pulau Pinang', 'Butterworth'),
('Malaysia', 'Pulau Pinang', 'Perai'),
('Malaysia', 'Pulau Pinang', 'Nibong Tebal'),
('Malaysia', 'Pulau Pinang', 'Permatang Kucing'),
('Malaysia', 'Pulau Pinang', 'Tanjung Tokong'),
('Malaysia', 'Pulau Pinang', 'Kepala Batas'),
('Malaysia', 'Pulau Pinang', 'Tanjung Bungah'),
('Malaysia', 'Pulau Pinang', 'Juru'),
('Malaysia', 'Sabah', 'Kota Kinabalu'),
('Malaysia', 'Sabah', 'Sandakan'),
('Malaysia', 'Sabah', 'Tawau'),
('Malaysia', 'Sabah', 'Lahad Datu'),
('Malaysia', 'Sabah', 'Keningau'),
('Malaysia', 'Sabah', 'Putatan'),
('Malaysia', 'Sabah', 'Donggongon'),
('Malaysia', 'Sabah', 'Semporna'),
('Malaysia', 'Sabah', 'Kudat'),
('Malaysia', 'Sabah', 'Kunak'),
('Malaysia', 'Sabah', 'Papar'),
('Malaysia', 'Sabah', 'Ranau'),
('Malaysia', 'Sabah', 'Beaufort'),
('Malaysia', 'Sabah', 'Kinarut'),
('Malaysia', 'Sabah', 'Kota Belud'),
('Malaysia', 'Sarawak', 'Kuching'),
('Malaysia', 'Sarawak', 'Miri'),
('Malaysia', 'Sarawak', 'Sibu'),
('Malaysia', 'Sarawak', 'Bintulu'),
('Malaysia', 'Sarawak', 'Limbang'),
('Malaysia', 'Sarawak', 'Sarikei'),
('Malaysia', 'Sarawak', 'Sri Aman'),
('Malaysia', 'Sarawak', 'Kapit'),
('Malaysia', 'Sarawak', 'Batu Delapan Bazaar'),
('Malaysia', 'Sarawak', 'Kota Samarahan'),
('Malaysia', 'Selangor', 'Subang Jaya'),
('Malaysia', 'Selangor', 'Klang'),
('Malaysia', 'Selangor', 'Ampang Jaya'),
('Malaysia', 'Selangor', 'Shah Alam'),
('Malaysia', 'Selangor', 'Petaling Jaya'),
('Malaysia', 'Selangor', 'Cheras'),
('Malaysia', 'Selangor', 'Kajang'),
('Malaysia', 'Selangor', 'Selayang Baru'),
('Malaysia', 'Selangor', 'Rawang'),
('Malaysia', 'Selangor', 'Taman Greenwood'),
('Malaysia', 'Selangor', 'Semenyih'),
('Malaysia', 'Selangor', 'Banting'),
('Malaysia', 'Selangor', 'Balakong'),
('Malaysia', 'Selangor', 'Gombak Setia'),
('Malaysia', 'Selangor', 'Kuala Selangor'),
('Malaysia', 'Selangor', 'Serendah'),
('Malaysia', 'Selangor', 'Bukit Beruntung'),
('Malaysia', 'Selangor', 'Pengkalan Kundang'),
('Malaysia', 'Selangor', 'Jenjarom'),
('Malaysia', 'Selangor', 'Sungai Besar'),
('Malaysia', 'Selangor', 'Batu Arang'),
('Malaysia', 'Selangor', 'Tanjung Sepat'),
('Malaysia', 'Selangor', 'Kuang'),
('Malaysia', 'Selangor', 'Kuala Kubu Baharu'),
('Malaysia', 'Selangor', 'Batang Berjuntai'),
('Malaysia', 'Selangor', 'Bandar Baru Salak Tinggi'),
('Malaysia', 'Selangor', 'Sekinchan'),
('Malaysia', 'Selangor', 'Sabak'),
('Malaysia', 'Selangor', 'Tanjung Karang'),
('Malaysia', 'Selangor', 'Beranang'),
('Malaysia', 'Selangor', 'Sungai Pelek'),
('Malaysia', 'Terengganu', 'Kuala Terengganu'),
('Malaysia', 'Terengganu', 'Chukai'),
('Malaysia', 'Terengganu', 'Dungun'),
('Malaysia', 'Terengganu', 'Kerteh'),
('Malaysia', 'Terengganu', 'Kuala Berang'),
('Malaysia', 'Terengganu', 'Marang'),
('Malaysia', 'Terengganu', 'Paka'),
('Malaysia', 'Terengganu', 'Jerteh'),
('Malaysia', 'Wilayah Persekutuan (KL)', 'Kuala Lumpur'),
('Malaysia', 'Wilayah Persekutuan (Labuan)', 'Labuan'),
('Malaysia', 'Wilayah Persekutuan (Putrajaya)', 'Putrajaya')
;

INSERT INTO StateProvince
(Name, Abbreviation, CountryId, Published, DisplayOrder)
select DISTINCT tl.`State`, '', c.Id, 1, 1 
from `tmpLocation` tl
inner join Country c
on c.Name  = tl.Country
where NOT EXISTS
	(SELECT 1
	FROM StateProvince sp
	where sp.Name = tl.`State`
	and sp.CountryId = c.Id 
	limit 1);

INSERT INTO City
(Name, Abbreviation, Published, DisplayOrder, CountryId, StateProvinceId)
select DISTINCT tl.`City`, '', 1, 1, c.Id, sp.Id 
from `tmpLocation` tl
inner join Country c
on c.Name  = tl.Country
inner join StateProvince sp
on sp.CountryId  = c.Id 
and sp.Name  = tl.`State`
where NOT EXISTS
	(SELECT 1
	FROM City ct
	where ct.Name = tl.`City`
	and ct.CountryId = c.Id 
	and ct.StateProvinceId  = sp.Id 
	limit 1);



DROP TABLE IF EXISTS `tmpLocation`;



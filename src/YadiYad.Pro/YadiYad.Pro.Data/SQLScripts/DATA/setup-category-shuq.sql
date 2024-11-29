DROP TEMPORARY TABLE IF EXISTS `tmpCategory`;

CREATE TEMPORARY TABLE `tmpCategory`(
  `Id` int(11) NOT NULL AUTO_INCREMENT
  ,`Name` varchar(200) NOT NULL
  ,`ParentCategoryId` INT(11) 
  ,PRIMARY KEY (`Id`)
);

INSERT INTO `tmpCategory` 
(`Name`, `ParentCategoryId`) VALUES 
('Shuq Eats', 0),
('Shuq Mart', 0)
;

INSERT INTO `Category`
(Name, DisplayOrder, Published, CategoryTemplateId, PictureId, ParentCategoryId, PageSize, AllowCustomersToSelectPageSize, ShowOnHomePage, IncludeInTopMenu, SubjectToAcl, LimitedToStores, Deleted, CreatedOnUTC, UpdatedOnUTC)
select `Name`, ROW_NUMBER()  OVER (Order By tlsr.`Id` ASC), 1, 1, 0, `ParentCategoryId`, 6, 1, 0, 1, 0, 0, 0, NOW(), NOW()
from `tmpCategory` tlsr 
where NOT EXISTS
	(SELECT 0
	FROM `Category` lsr
	where tlsr.Name = lsr.Name 
	limit 1);

TRUNCATE TABLE tmpCategory;

INSERT INTO `tmpCategory` 
(`Name`, `ParentCategoryId`) VALUES 
('Fresh', (SELECT Id from Category where Name="Shuq Eats")),
('Frozen', (SELECT Id from Category where Name="Shuq Eats")),
('Storable Perishable', (SELECT Id from Category where Name="Shuq Mart")),
('Non-Perishable', (SELECT Id from Category where Name="Shuq Mart"))
;

INSERT INTO `Category`
(Name, DisplayOrder, Published, CategoryTemplateId, PictureId, ParentCategoryId, PageSize, AllowCustomersToSelectPageSize, ShowOnHomePage, IncludeInTopMenu, SubjectToAcl, LimitedToStores, Deleted, CreatedOnUTC, UpdatedOnUTC)
select `Name`, ROW_NUMBER()  OVER (Order By tlsr.`Id` ASC), 1, 1, 0, `ParentCategoryId`, 6, 1, 0, 1, 0, 0, 0, NOW(), NOW()
from `tmpCategory` tlsr 
where NOT EXISTS
	(SELECT 0
	FROM `Category` lsr
	where tlsr.Name = lsr.Name 
	limit 1);
	
TRUNCATE TABLE tmpCategory;

INSERT INTO `tmpCategory` 
(`Name`, `ParentCategoryId`) VALUES 
('Local Specialties & Delights', (SELECT Id from Category where Name="Fresh")),
('Cuisine by Region / Country', (SELECT Id from Category where Name="Fresh")),
('Fusion Cuisine', (SELECT Id from Category where Name="Fresh")),
('Vegan Cuisine', (SELECT Id from Category where Name="Fresh")),
('Special Dietary Cuisine', (SELECT Id from Category where Name="Fresh")),
('Cakes & Pastries', (SELECT Id from Category where Name="Fresh")),
('Sweets & Desserts', (SELECT Id from Category where Name="Fresh")),
('Beverages', (SELECT Id from Category where Name="Fresh")),
('Festive & Seasonal Delicacies', (SELECT Id from Category where Name="Fresh")),
('Dim Sum', (SELECT Id from Category where Name="Frozen")),
('Dumpling', (SELECT Id from Category where Name="Frozen")),
('Dairy', (SELECT Id from Category where Name="Frozen")),
('Finger Foods', (SELECT Id from Category where Name="Frozen")),
('Festive & Seasonal Delicacies', (SELECT Id from Category where Name="Frozen")),
('Frozen Pastries', (SELECT Id from Category where Name="Frozen")),
('Patties & Sausages', (SELECT Id from Category where Name="Frozen")),
('Vegan / Vegetarian', (SELECT Id from Category where Name="Frozen")),
('Yong Tau Fu', (SELECT Id from Category where Name="Frozen")),
('Biscuits & Cookies', (SELECT Id from Category where Name="Storable Perishable")),
('Condiments & Sauces', (SELECT Id from Category where Name="Storable Perishable")),
('Coffee & Tea', (SELECT Id from Category where Name="Storable Perishable")),
('Candies', (SELECT Id from Category where Name="Storable Perishable")),
('Dried Fruits', (SELECT Id from Category where Name="Storable Perishable")),
('Jams & Spreads', (SELECT Id from Category where Name="Storable Perishable")),
('Spices & Herbs', (SELECT Id from Category where Name="Storable Perishable")),
('Snacks', (SELECT Id from Category where Name="Storable Perishable")),
('Artwork & Paintings', (SELECT Id from Category where Name="Non-Perishable")),
('Beauty Products', (SELECT Id from Category where Name="Non-Perishable")),
('Fashion Apparels & Wearables', (SELECT Id from Category where Name="Non-Perishable")),
('Household Decor', (SELECT Id from Category where Name="Non-Perishable")),
('Handicrafts', (SELECT Id from Category where Name="Non-Perishable")),
('Jewelleries & Ornaments', (SELECT Id from Category where Name="Non-Perishable")),
('Toys & Playthings', (SELECT Id from Category where Name="Non-Perishable"))
;

INSERT INTO `Category`
(Name, DisplayOrder, Published, CategoryTemplateId, PictureId, ParentCategoryId, PageSize, AllowCustomersToSelectPageSize, ShowOnHomePage, IncludeInTopMenu, SubjectToAcl, LimitedToStores, Deleted, CreatedOnUTC, UpdatedOnUTC)
select `Name`, ROW_NUMBER()  OVER (Order By tlsr.`Id` ASC), 1, 1, 0, `ParentCategoryId`, 6, 1, 0, 1, 0, 0, 0, NOW(), NOW()
from `tmpCategory` tlsr 
where NOT EXISTS
	(SELECT 0
	FROM `Category` lsr
	where tlsr.Name = lsr.Name 
	limit 1);

DROP TABLE IF EXISTS `tmpCategory`;




SELECT * FROM Category Order BY id;

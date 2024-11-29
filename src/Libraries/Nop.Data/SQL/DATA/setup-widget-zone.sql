
DROP TEMPORARY TABLE IF EXISTS `tmpWidgetZone`;

CREATE TEMPORARY TABLE `tmpWidgetZone`(
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) NOT NULL,
  `SystemName` varchar(200) NOT NULL,
  `TransitionEffects` longtext DEFAULT NULL,
  `AutoPlayInterval` int(11) NOT NULL,
  `SlideDuration` int(11) NOT NULL,
  `MinDragOffsetToSlide` int(11) NOT NULL,
  `SlideSpacing` int(11) NOT NULL,
  `MinSlideWidgetZoneWidth` int(11) NOT NULL,
  `MaxSlideWidgetZoneWidth` int(11) NOT NULL,
  `AutoPlay` tinyint(1) NOT NULL,
  `ArrowNavigationDisplayingTypeId` int(11) NOT NULL,
  `BulletNavigationDisplayingTypeId` int(11) NOT NULL,
  `Published` tinyint(1) NOT NULL,
  `SubjectToAcl` tinyint(1) NOT NULL,
  `LimitedToStores` tinyint(1) NOT NULL,
  `MaxSlideWidgetZoneHeight` int(11) DEFAULT NULL,
  `MinSlideWidgetZoneHeight` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8;

INSERT INTO `tmpWidgetZone` (Name,SystemName,TransitionEffects,AutoPlayInterval,SlideDuration,MinDragOffsetToSlide,SlideSpacing,MinSlideWidgetZoneWidth,MaxSlideWidgetZoneWidth,AutoPlay,ArrowNavigationDisplayingTypeId,BulletNavigationDisplayingTypeId,Published,SubjectToAcl,LimitedToStores,MaxSlideWidgetZoneHeight,MinSlideWidgetZoneHeight) VALUES
	 ('Main homepage slider','home_page_top',NULL,3000,500,20,0,200,1920,1,0,0,1,0,0,NULL,NULL),
	 ('Shuq Mart','shuq_mart_page_top',NULL,3000,500,20,0,200,1200,1,0,2,1,0,0,NULL,NULL),
	 ('Shuq Vendor Page','shuq_vendor_page',NULL,3000,500,20,0,200,1920,1,1,2,1,0,0,600,200),
	 ('Shuq Eat','shuq_eat_page_top',NULL,3000,500,20,0,200,1200,1,0,2,1,0,0,NULL,NULL),
	 ('Shuq Eat Side Banner','shuq_eat_page_side',NULL,3000,500,20,0,200,1920,1,0,0,1,0,0,NULL,200),
	 ('Shuq Mart Side Banner','shuq_mart_page_side',NULL,3000,500,20,0,200,1920,1,0,0,1,0,0,NULL,NULL);
	 
INSERT INTO Baroque_qBoSlider_WidgetZone
(Name,SystemName,TransitionEffects,AutoPlayInterval,SlideDuration,MinDragOffsetToSlide,SlideSpacing,MinSlideWidgetZoneWidth,MaxSlideWidgetZoneWidth,AutoPlay,ArrowNavigationDisplayingTypeId,BulletNavigationDisplayingTypeId,Published,SubjectToAcl,LimitedToStores,MaxSlideWidgetZoneHeight,MinSlideWidgetZoneHeight)
select Name,SystemName,TransitionEffects,AutoPlayInterval,SlideDuration,MinDragOffsetToSlide,SlideSpacing,MinSlideWidgetZoneWidth,MaxSlideWidgetZoneWidth,AutoPlay,ArrowNavigationDisplayingTypeId,BulletNavigationDisplayingTypeId,Published,SubjectToAcl,LimitedToStores,MaxSlideWidgetZoneHeight,MinSlideWidgetZoneHeight
from `tmpWidgetZone` tlsr 
where NOT EXISTS
	(SELECT 0
	FROM Baroque_qBoSlider_WidgetZone lsr
	where tlsr.SystemName = lsr.SystemName 
	limit 1);
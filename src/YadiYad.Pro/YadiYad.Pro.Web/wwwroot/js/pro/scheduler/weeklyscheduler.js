'use strict';

/* eslint-disable */
/* eslint-env jquery */
/* global moment, tui, chance */
/* global findCalendar, CalendarList, ScheduleList, generateSchedule */

var WeeklyScheduler = function (containerSelector, isReadOnly, viewMode, customDateRange, weekOptions) {
    var base = this;
    var Calendar = tui.Calendar;
    var ScheduleList = [];

    var cal, resizeThrottled;
    var useCreationPopup = false;
    var useDetailPopup = true && !isReadOnly;
    var datePicker, selectedCalendar;
    var isAllDay = false;

    const VIEW = {
        MONTHLY: "monthly",
        WEEKLY: "weekly",
        WEEK: "week"
    };

    if (viewMode === VIEW.MONTHLY) {
        isAllDay = true;
    }

    var enabledDateRange = {
        minDate: null,
        maxDate: null
    };

    cal = new Calendar(containerSelector, {
        isReadOnly: isReadOnly && true,
        usageStatistics: false,
        defaultView: viewMode && viewMode == VIEW.MONTHLY?'month':"week",
        useCreationPopup: useCreationPopup,
        useDetailPopup: useDetailPopup,
        calendars: CalendarList,
        taskView: [],
        scheduleView: ['time'],
        template: {
            milestone: function (model) {
                return '<span class="calendar-font-icon ic-milestone-b"></span> <span style="background-color: ' + model.bgColor + '">' + model.title + '</span>';
            },
            allday: function (schedule) {
                return getTimeTemplate(schedule, true);
            },
            time: function (schedule) {
                return getTimeTemplate(schedule, false);
            }
        },
        week: {
            customDateRange: customDateRange,
            hourStart: (weekOptions && weekOptions.hourStart)||0,
            hourEnd: (weekOptions && weekOptions.hourEnd)||24
        }
    });

    Object.defineProperty(base, 'Calendar', {
        get() {
            return cal;
        }
    });

    $(containerSelector).on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
    });

    var isWithinAllowedDateRange = function (e) {
        var minDate = enabledDateRange.minDate && new Date(enabledDateRange.minDate);
        var maxDate = enabledDateRange.maxDate && new Date(enabledDateRange.maxDate);
        if (minDate && maxDate) {
            maxDate.setDate(maxDate.getDate() + 1);

            //stop create new schedule if selected date not in allowed date range
            if ((minDate && minDate > e.start._date)
                || (maxDate && maxDate < e.start._date)
                || (minDate && minDate > e.end._date)
                || (maxDate && maxDate < e.end._date)) {
                if (e.guide) {
                    e.guide.clearGuideElement();
                }
                return false;
            }
        }
        return true;
    }

    function dateRangeOverlaps(a_start, a_end, b_start, b_end) {
        if (a_start < b_start && b_start < a_end) return true; // b starts in a
        if (a_start < b_end && b_end < a_end) return true; // b ends in a
        if (b_start < a_start && a_end < b_end) return true; // a in b
        if (b_start.getTime() === a_start.getTime() && a_end.getTime() === b_end.getTime()) return true; // a in b
        return false;
    }

    var notOverlapSchedule = function (e) {
        var isScheduleOverlapped = false;

        $.each(cal._controller.schedules.items, function (i, item) {
            if (dateRangeOverlaps(e.start._date, e.end._date, item.start._date, item.end._date)
                && (!e.schedule || (e.schedule && e.schedule.id !== item.id))) {
                isScheduleOverlapped = true;

                return false;
            }
        });

        return isScheduleOverlapped === false;
    }

    // event handlers
    cal.on({
        'clickMore': function (e) {
            console.log('clickMore', e);
        },
        'clickSchedule': function (e) {
            console.log('clickSchedule', e);
        },
        'clickDayname': function (date) {
            console.log('clickDayname', date);
        },
        'beforeCreateSchedule': function (e) {
            console.log('beforeCreateSchedule', e);

            if (isWithinAllowedDateRange(e) && notOverlapSchedule(e)) {
                saveNewSchedule(e);
                base.onChanged && base.onChanged(e);
            }
            else {
                refreshScheduleVisibility();
            }
        },
        'beforeUpdateSchedule': function (e) {
            var schedule = e.schedule;
            var changes = e.changes;

            if (!changes) {
                return true;
            }

            console.log('beforeUpdateSchedule', e);

            if (changes && !changes.isAllDay && schedule.category === 'allday') {
                changes.category = 'time';
            }

            if (isWithinAllowedDateRange(e) && notOverlapSchedule(e)) {
                cal.updateSchedule(schedule.id, schedule.calendarId, changes);
            }
            else {
                cal.deleteSchedule(e.schedule.id, e.schedule.calendarId);
            }

            refreshScheduleVisibility();

            base.onChanged && base.onChanged(e);
        },
        'beforeDeleteSchedule': function (e) {
            console.log('beforeDeleteSchedule', e);
            cal.deleteSchedule(e.schedule.id, e.schedule.calendarId);

            base.onChanged && base.onChanged(e);
        },
        'afterRenderSchedule': function (e) {
            var schedule = e.schedule;
            var element = cal.getElement(schedule.id, schedule.calendarId);
            console.log('afterRenderSchedule', element);
        },
        'afterRender': function (e) {
            console.log('afterRender', e);
        },
        'clickTimezonesCollapseBtn': function (timezonesCollapsed) {
            console.log('timezonesCollapsed', timezonesCollapsed);

            if (timezonesCollapsed) {
                cal.setTheme({
                    'week.daygridLeft.width': '77px',
                    'week.timegridLeft.width': '77px'
                });
            } else {
                cal.setTheme({
                    'week.daygridLeft.width': '60px',
                    'week.timegridLeft.width': '60px'
                });
            }

            return true;
        }
    });


    /**
     * Get time template for time and all-day
     * @param {Schedule} schedule - schedule
     * @param {boolean} isAllDay - isAllDay or hasMultiDates
     * @returns {string}
     */
    function getTimeTemplate(schedule, isAllDay) {
        var html = [];
        var start = moment(schedule.start.toUTCString());
        if (!isAllDay) {
            html.push('<strong>' + start.format('HH:mm') + '</strong> ');
        }
        if (schedule.isPrivate) {
            html.push('<span class="calendar-font-icon ic-lock-b"></span>');
            html.push(' Private');
        } else {
            if (schedule.isReadOnly) {
                html.push('<span class="calendar-font-icon ic-readonly-b"></span>');
            } else if (schedule.recurrenceRule) {
                html.push('<span class="calendar-font-icon ic-repeat-b"></span>');
            } else if (schedule.attendees.length) {
                html.push('<span class="calendar-font-icon ic-user-b"></span>');
            } else if (schedule.location) {
                html.push('<span class="calendar-font-icon ic-location-b"></span>');
            }
            html.push(' ' + schedule.title);
        }

        return html.join('');
    }

    function onClickNavi(e) {
        var action = getDataAction(e.target);

        switch (action) {
            case 'move-prev':
                cal.prev();
                break;
            case 'move-next':
                cal.next();
                break;
            case 'move-today':
                cal.today();
                break;
            default:
                return;
        }

        setRenderRangeText();
        setSchedules();
    }

    function onChangeNewScheduleCalendar(e) {
        var target = $(e.target).closest('a[role="menuitem"]')[0];
        var calendarId = getDataAction(target);
        changeNewScheduleCalendar(calendarId);
    }

    function changeNewScheduleCalendar(calendarId) {
        var calendarNameElement = document.getElementById('calendarName');
        var calendar = findCalendar(calendarId);
        var html = [];

        html.push('<span class="calendar-bar" style="background-color: ' + calendar.bgColor + '; border-color:' + calendar.borderColor + ';"></span>');
        html.push('<span class="calendar-name">' + calendar.name + '</span>');

        calendarNameElement.innerHTML = html.join('');

        selectedCalendar = calendar;
    }

    function saveNewSchedule(scheduleData) {
        var calendar = scheduleData.calendar || findCalendar(scheduleData.calendarId);
        var schedule = {
            id: String(chance.guid()),
            title: scheduleData.title,
            isAllDay: scheduleData.isAllDay,
            start: scheduleData.start,
            end: scheduleData.end,
            category: scheduleData.isAllDay ? 'allday' : 'time',
            dueDateClass: '',
            bgColor: calendar.bgColor,
            dragBgColor: calendar.bgColor,
            borderColor: calendar.borderColor,
            location: scheduleData.location,
            raw: {
                //class: scheduleData.raw['class']
            },
            state: scheduleData.state
        };
        if (calendar) {
            schedule.calendarId = calendar.id;
            schedule.color = calendar.color;
            schedule.bgColor = calendar.bgColor;
            schedule.borderColor = calendar.borderColor;
        }

        cal.createSchedules([schedule]);

        refreshScheduleVisibility();
    }

    function refreshScheduleVisibility() {
        var calendarElements = Array.prototype.slice.call(document.querySelectorAll('#calendarList input'));

        CalendarList.forEach(function (calendar) {
            cal.toggleSchedules(calendar.id, !calendar.checked, false);
        });

        cal.render(true);

        calendarElements.forEach(function (input) {
            var span = input.nextElementSibling;
            span.style.backgroundColor = input.checked ? span.style.borderColor : 'transparent';
        });
    }

    function currentCalendarDate(format) {
        var currentDate = moment([cal.getDate().getFullYear(), cal.getDate().getMonth(), cal.getDate().getDate()]);

        return currentDate.format(format);
    }

    function setRenderRangeText() {
        var renderRange = document.getElementById('renderRange');
        if (renderRange) {
            var options = cal.getOptions();
            var viewName = cal.getViewName();

            var html = [];
            if (viewName === 'day') {
                html.push(currentCalendarDate('DD MMM YYYY'));
            } else if (viewName === 'month' &&
                (!options.month.visibleWeeksCount || options.month.visibleWeeksCount > 4)) {
                html.push(currentCalendarDate('MMM YYYY'));
            } else {
                html.push(moment(cal.getDateRangeStart().getTime()).format('DD MMM YYYY'));
                html.push(' - ');
                html.push(moment(cal.getDateRangeEnd().getTime()).format('DD MMM YYYY'));
            }
            renderRange.innerHTML = html.join('');
        }
    }

    function setSchedules() {
        //cal.clear();

        refreshScheduleVisibility();
    }

    function setEventListener() {
        $('#menu-navi').on('click', onClickNavi);

        $('#dropdownMenu-calendars-list').on('click', onChangeNewScheduleCalendar);

        window.addEventListener('resize', resizeThrottled);
    }

    function getDataAction(target) {
        return target.dataset ? target.dataset.action : target.getAttribute('data-action');
    }

    resizeThrottled = tui.util.throttle(function () {
        cal.render();
    }, 50);

    base.setDate = function (startDate) {
        cal.setDate(startDate);
    };

    base.setEnabledDateRange = function (mode, minDate, maxDate) {
        var options = cal.getOptions();
        enabledDateRange.minDate = minDate;
        enabledDateRange.maxDate = maxDate;

        options.customDateRange = {
            renderStartDate: minDate,
            renderEndDate: maxDate,
        };
        base.setViewMode(mode);
    };

    base.setSchedule = function (scheduleDatas, stopNavigateToDate) {
        var calendar = findCalendar(undefined);
        cal.clear();
        var scheduleList = [];
        var startDates = [];

        scheduleDatas.forEach(function (scheduleData, i) {

            var schedule = {
                id: String(chance.guid()),
                isAllDay: isAllDay,
                isReadOnly: scheduleData.isReadOnly && true,
                start: scheduleData.start,
                end: scheduleData.end,
                category: isAllDay ? 'allday' : 'time',
                dueDateClass: '',
                color: calendar.color,
                bgColor: calendar.bgColor,
                dragBgColor: calendar.bgColor,
                borderColor: calendar.borderColor,
                raw: {
                },
            };
            startDates.push(scheduleData.start);
            scheduleList.push(schedule);
        });

        startDates = startDates.sort(function (a, b) {
            return new Date(b.date) - new Date(a.date);
        });

        var calendarDate = startDates[0]

        cal.createSchedules(scheduleList);

        if (!stopNavigateToDate) {
            cal.setDate(calendarDate);
        }

        refreshScheduleVisibility();


        var time = moment(calendarDate).format('h a');

        var matchedTime = $('.tui-full-calendar-timegrid-hour').filter(function () {
            return $(this).text().trim() === time
        });

        //scroll to scheduled time
        if (matchedTime && matchedTime.length > 0) {
            var offsetTop = matchedTime[0].offsetTop - 20;
            setTimeout(function () {
                $('.tui-full-calendar-timegrid-container').scrollTop(offsetTop);
            }, 300);
        }
    }

    base.setViewMode = function (mode) {
        var options = cal.getOptions();
        var viewName = '';

        $(containerSelector)
            .removeClass('tui-layout-' + VIEW.WEEKLY)
            .removeClass('tui-layout-' + VIEW.WEEK)
            .removeClass('tui-layout-' + VIEW.MONTHLY);

        switch (mode) {
            case VIEW.WEEKLY:
                $(containerSelector).addClass('tui-layout-'+VIEW.WEEKLY);
                viewName = 'week';
                isAllDay = false;
                break;
            case VIEW.WEEK:
                viewName = 'week';
                isAllDay = false;
                $(containerSelector).addClass('tui-layout-' +VIEW.WEEK);
                break;
            case VIEW.MONTHLY:
                $(containerSelector).addClass('tui-layout-' +VIEW.MONTHLY);
                options.month.visibleWeeksCount = 0;
                viewName = 'month';
                isAllDay = true;
                break;
            default:
                break;
        }

        cal.setOptions(options, true);
        cal.changeView(viewName, true);

        refreshScheduleVisibility();
    };

    window.cal = cal;

    if (viewMode === VIEW.WEEK) {
        var startDate = new Date();
        startDate.setYear(1990);
        startDate.setMonth(1 - 1);
        startDate.setDate(1);
        startDate.setHours(0, 0, 0, 0);

        var endDate = new Date();
        endDate.setYear(1990);
        endDate.setMonth(1 - 1);
        endDate.setDate(7);
        endDate.setHours(23, 59, 59, 99);
        base.setEnabledDateRange('week', startDate, endDate);
    }

    setRenderRangeText();
    setSchedules();
    setEventListener();
};

function CategoryViewModel() {
    var self = this;
    self.coursecategory = ko.observableArray();
    self.courseAvailability = ko.observableArray();
    self.courseLevel = ko.observableArray();
    self.courseLanguage = ko.observableArray();
    self.courseType = ko.observableArray();
    self.courses = ko.observableArray();
    self.freecourses = ko.observableArray();

    GetCourses(self, null, null, null, null);
    $("#writecontentsection").show();
    $('#CoursesSection').hide();
    var coursecategory = [];
    $.ajax({
        type: "POST",
        async: true,
        data: {},
        contentType: 'application/json; charset=utf-8',
        url: "/Course/GetCourseCategoryList",
        success: function (response) {

            self.coursecategory.removeAll();
            for (var i = 0; i < response.length; i++) {
                coursecategory.push(JSON.parse(response[i]));
            }
            self.coursecategory(coursecategory);
            $("#writecontentsection").hide();
            $('#CoursesSection').show();

        },
        error: function (response) {

        }

    });
    var courseAvailability = [];
    $('#writecontentsection').show();
    $('#CoursesSection').hide();
    $.ajax({
        type: "POST",
        async: true,
        data: {},
        contentType: 'application/json; charset=utf-8',
        url: "/Course/GetCourseAvailability",
        success: function (response) {

            self.courseAvailability.removeAll();
            for (var i = 0; i < response.length; i++) {
                courseAvailability.push(JSON.parse(response[i]));
            }
            self.courseAvailability(courseAvailability);
            $('#writecontentsection').hide();
            $('#CoursesSection').show();
        },
        error: function (response) {

        }

    });
    var courseLevel = [];
    $('#writecontentsection').show();
    $('#CoursesSection').hide();
    $.ajax({
        type: "POST",
        async: true,
        data: {},
        contentType: 'application/json; charset=utf-8',
        url: "/Course/GetCourseLevel",
        success: function (response) {

            self.courseLevel.removeAll();
            for (var i = 0; i < response.length; i++) {
                courseLevel.push(JSON.parse(response[i]));
            }
            self.courseLevel(courseLevel);
            $('#writecontentsection').hide();
            $('#CoursesSection').show();
        },
        error: function (response) {

        }

    });

    var courseLanguage = [];
    $('#writecontentsection').show();
    $('#CoursesSection').hide();
    $.ajax({
        type: "POST",
        async: true,
        data: {},
        contentType: 'application/json; charset=utf-8',
        url: "/Course/GetCourseLanguage",
        success: function (response) {

            self.courseLanguage.removeAll();
            for (var i = 0; i < response.length; i++) {
                courseLanguage.push(JSON.parse(response[i]));
            }
            self.courseLanguage(courseLanguage);
            $('#writecontentsection').hide();
            $('#CoursesSection').show();
        },
        error: function (response) {

        }

    });

    var courseType = [];
    $('#writecontentsection').show();
    $('#CoursesSection').hide();
    $.ajax({
        type: "POST",
        async: true,
        data: {},
        contentType: 'application/json; charset=utf-8',
        url: "/Course/GetCourseType",
        success: function (response) {

            self.courseType.removeAll();
            for (var i = 0; i < response.length; i++) {
                courseType.push(JSON.parse(response[i]));
            }
            self.courseType(courseType);
            $('#writecontentsection').hide();
            $('#CoursesSection').show();
        },
        error: function (response) {

        }

    });

    var courses = [];
    self.displayCourses = function () {

        var currentCourse = this;
        GetCourses(self, currentCourse.Id, currentCourse.Type, null, null)
    };
    var fbCourses = [];
    self.getcurrentCourse = function () {
        var currentselectedCourses = this;
        postToWallUsingFBUi(self, currentselectedCourses.tempCourseName, currentselectedCourses.BasePrice, currentselectedCourses.ShortDescription, currentselectedCourses.Id, currentselectedCourses.CourseImageLink)
    };
    var linkincourses = [];
    self.linkcourse = function () {
        var linkincourse = this;
        var url = 'https://www.linkedin.com/cws/share?mini=true&url=' + linkincourse.GoogleShareURL + '&title=' + linkincourse.CourseName + '&summary=' + linkincourse.ShortDescription + '&source=Millionlights';
        location.href = url;
    };
    var googleSCourses = [];
    self.googleShareCourse = function (event) {
        var gcourse = this;

        var w = 480; var h = 380;

        var x = Number((window.screen.width - w) / 2);

        var y = Number((window.screen.height - h) / 2);

        window.open('https://plusone.google.com/_/+1/confirm?hl=en&url=' + decodeURIComponent(gcourse.GoogleShareURL) +
         '&title=' + decodeURIComponent(gcourse.CourseName), '', 'width=' + w + ',height=' + h + ',left=' + x + ',top=' + y + ',scrollbars=no');
    };
    var emailCourses = [];
    self.emailCourse = function () {
        var ecourse = this;
        var url = 'mailto:?subject=' + ecourse.CourseName + '&body=' + ecourse.EmailCourseName;
        location.href = url;
    };

    self.searchCoursesByPriceRange = function () {
        var minValue = $('#min')[0].value;
        var maxValue = $('#max')[0].value;
        var freeCourses = [];
        var paidCourses = [];
        $('#writecontentsection').show();
        $('#CoursesSection').hide();
        $.ajax({
            type: "POST",
            async: true,
            data: JSON.stringify({ clickedId: null, type: "FilterCourse", MinPrice: minValue, MaxPrice: maxValue }),
            contentType: 'application/json; charset=utf-8',
            url: "/Course/GetCoursesByCategoryId",
            success: function (response) {
                self.courses.removeAll();
                self.freecourses.removeAll();
                for (var i = 0; i < response.length; i++) {

                    if (JSON.parse(response[i]).CoursePrice == "Free") {
                        freeCourses.push(JSON.parse(response[i]));
                    }
                    else {
                        paidCourses.push(JSON.parse(response[i]));
                    }

                }
                self.courses(paidCourses);
                self.freecourses(freeCourses);
                $('#writecontentsection').hide();
                $('#CoursesSection').show();
                $("#loderimage").hide();
                $("#main_content").show();
            },
            error: function (data) {

            }

        });
    };
    self.searchFreeCourses = function () {
        var freeCourses = [];
        var paidCourses = [];
        $('#writecontentsection').show();
        $('#CoursesSection').hide();
        $.ajax({
            type: "POST",
            async: true,
            data: JSON.stringify({ clickedId: null, type: "FreeCourse", MinPrice: null, MaxPrice: null }),
            contentType: 'application/json; charset=utf-8',
            url: "/Course/GetCoursesByCategoryId",
            success: function (response) {
                self.courses.removeAll();
                self.freecourses.removeAll();
                for (var i = 0; i < response.length; i++) {

                    if (JSON.parse(response[i]).CoursePrice == "Free") {
                        freeCourses.push(JSON.parse(response[i]));
                    }
                    else {
                        paidCourses.push(JSON.parse(response[i]));
                    }

                }
                self.courses(paidCourses);
                self.freecourses(freeCourses);
                $('#writecontentsection').hide();
                $('#CoursesSection').show();
            },
            error: function (data) {

            }
        });
    };
}

function GetCourses(self, courseId, tempType, minPrice, maxPrice) {
    var freeCourses = [];
    var paidCourses = [];
    $('#writecontentsection').show();
    $('#CoursesSection').hide();
    $.ajax({
        type: "POST",
        async: true,
        data: JSON.stringify({ clickedId: courseId, type: tempType, MinPrice: minPrice, MaxPrice: maxPrice }),
        contentType: 'application/json; charset=utf-8',
        url: "/Course/GetCoursesByCategoryId",
        success: function (response) {

            self.courses.removeAll();
            self.freecourses.removeAll();
            for (var i = 0; i < response.length; i++) {

                if (JSON.parse(response[i]).CoursePrice == "Free") {
                    freeCourses.push(JSON.parse(response[i]));
                }
                else
                {
                    paidCourses.push(JSON.parse(response[i]));
                }

            }
            self.courses(paidCourses);
            self.freecourses(freeCourses);
            $('#writecontentsection').hide();
            $('#CoursesSection').show();
            $("#loderimage").hide();
            $("#main_content").show();
        },
        error: function (data) {

        }

    });
}
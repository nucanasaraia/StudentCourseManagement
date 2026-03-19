using AutoMapper;
using StudentCourseManagement.DTOs;
using StudentCourseManagement.Models;
using StudentCourseManagement.Requests;

namespace StudentCourseManagement.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {   // Students
            CreateMap<User, UserDto>().ReverseMap();
            //.ForMember(dest => dest.Enrollments, opt => opt.MapFrom(src => src.Enrollments ?? new List<Enrollment>()));
            CreateMap<AddUser, User>();

            // Courses
            CreateMap<Course, CourseDto>().ReverseMap();
            CreateMap<AddCourse, Course>();

            // Enrollments
            CreateMap<AddEnrollment, Enrollment>();
            CreateMap<Enrollment, EnrollmentDto>();
        }
    }
}

using Database;
using EvidencePrihlasekWeb.Models;

namespace EvidencePrihlasekWeb.Services
{
    public class DashboardService
    {
        public async Task<DashboardModel> GetDashboardDataAsync(string username)
        {
            Users user = await DatabaseManager.GetUserByUsername<Users>(username);
            Student student = await DatabaseManager.GetEntityById<Student>(user.Id, "UserId");

            DashboardModel model = new DashboardModel();

            if (student == null)
            {
                model.ApplicationSubmitted = false;
                return model;
            }

            Application application = await DatabaseManager.GetEntityById<Application>(student.Id, "StudentId");

            model.ApplicationSubmitted = true;
            model.FirstName = student.FirstName;
            model.LastName = student.LastName;
            model.BirthDate = student.BirthDate;
            model.SubmissionDate = application.SubmissionDate;
            model.ContactPhone = student.ContactPhone ?? "Nezadáno";

            StudyProgram studyProgram1 = new StudyProgram();
            StudyProgram studyProgram2 = new StudyProgram();
            StudyProgram studyProgram3 = new StudyProgram();
            HighSchool highSchool1 = new HighSchool();
            HighSchool highSchool2 = new HighSchool();
            HighSchool highSchool3 = new HighSchool();

            if (application.StudyProgramId1 != 0 && application.StudyProgramId1.HasValue)
            {
                studyProgram1 = await DatabaseManager.GetEntityById<StudyProgram>((int)application.StudyProgramId1, "Id");
                highSchool1 = await DatabaseManager.GetEntityById<HighSchool>(studyProgram1.HighSchoolId, "Id");
            }

            if (application.StudyProgramId2 != 0 && application.StudyProgramId2.HasValue)
            {
                studyProgram2 = await DatabaseManager.GetEntityById<StudyProgram>((int)application.StudyProgramId2, "Id");
                highSchool2 = await DatabaseManager.GetEntityById<HighSchool>(studyProgram2.HighSchoolId, "Id");
            }

            if (application.StudyProgramId3 != 0 && application.StudyProgramId3.HasValue)
            {
                studyProgram3 = await DatabaseManager.GetEntityById<StudyProgram>((int)application.StudyProgramId3, "Id");
                highSchool3 = await DatabaseManager.GetEntityById<HighSchool>(studyProgram3.HighSchoolId, "Id");
            }

            model.SchoolAndProgram1 = Tuple.Create(highSchool1, studyProgram1);
            model.SchoolAndProgram2 = Tuple.Create(highSchool2, studyProgram2);
            model.SchoolAndProgram3 = Tuple.Create(highSchool3, studyProgram3);

            return model;
        }
    }
}

using TallyConnector.Core.Models.Masters.Payroll;

namespace TallyConnector.Services;
public partial class TallyService
{
    public async Task<AttendanceTypType> GetAttendanceTypeAsync<AttendanceTypType>(string LookupValue,
                                                                                   MasterRequestOptions? attendanceTypeOptions = null) where AttendanceTypType : AttendanceType
    {
        return await GetObjectAsync<AttendanceTypType>(LookupValue, attendanceTypeOptions);
    }
    public async Task<TallyResult> PostAttendanceTypeAsync<AttendanceTypType>(AttendanceTypType attendanceType,
                                                           PostRequestOptions? postRequestOptions = null) where AttendanceTypType : AttendanceType
    {

        return await PostObjectToTallyAsync(attendanceType, postRequestOptions);
    }
    public async Task<EmployeeGroupType> GetEmployeeGroupAsync<EmployeeGroupType>(string LookupValue,
                                                                                   MasterRequestOptions? employeeGroupOptions = null) where EmployeeGroupType : EmployeeGroup
    {
        return await GetObjectAsync<EmployeeGroupType>(LookupValue, employeeGroupOptions);
    }
    public async Task<TallyResult> PostEmployeeGroupAsync<EmployeeGroupType>(EmployeeGroupType employeeGroup,
                                                           PostRequestOptions? postRequestOptions = null) where EmployeeGroupType : EmployeeGroup
    {

        return await PostObjectToTallyAsync(employeeGroup, postRequestOptions);
    }


    public async Task<EmployeeType> GetEmployeeAsync<EmployeeType>(string LookupValue,
                                                                                   MasterRequestOptions? employeeOptions = null) where EmployeeType : Employee
    {
        return await GetObjectAsync<EmployeeType>(LookupValue, employeeOptions);
    }
    public async Task<TallyResult> PostEmployeeAsync<EmployeeType>(EmployeeType employee,
                                                           PostRequestOptions? postRequestOptions = null) where EmployeeType : Employee
    {

        return await PostObjectToTallyAsync(employee, postRequestOptions);
    }



}

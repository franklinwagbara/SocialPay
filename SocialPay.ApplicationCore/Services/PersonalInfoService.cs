﻿using AutoMapper;
using SocialPay.ApplicationCore.Interfaces.Repositories;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Domain.Entities;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Services
{
    
    public class PersonalInfoService : IPersonalInfoService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<ClientAuthentication> _clientAuthentication;

        public PersonalInfoService(IAsyncRepository<ClientAuthentication> clientAuthentication)
        {
            _clientAuthentication = clientAuthentication;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<ClientAuthentication, PersonalInfoViewModel>());

            _mapper = config.CreateMapper();
        }

        //public async Task<List<DepartmentViewModel>> GetAllAsync()
        //{
        //    var departments = await _department.GetAllAsync();

        //    return _mapper.Map<List<Department>, List<DepartmentViewModel>>(departments);
        //}

        public async Task<PersonalInfoViewModel> GetMerchantPersonalInfo(long clientId)
        {
            var personalInfo = await _clientAuthentication.GetSingleAsync(x => x.ClientAuthenticationId == clientId);

            return _mapper.Map<ClientAuthentication, PersonalInfoViewModel>(personalInfo);
        }

        //public async Task<bool> ExistsAsync(long departmentId)
        //{
        //    return await _department.ExistsAsync(x => x.DepartmentId == departmentId);
        //}


        //GetTenantByStatusAsync
        //public async Task<List<CreateInvestment>> GetByNameAsync(string name)
        //{
        //    var employees = await _investRepository.GetAsync(x => x..Contains(name));
        //    return _mapper.Map<List<InvestmentSetup>, List<CreateInvestment>>(employees);
        //}

        //public async Task<List<CreateInvestment>> GetAsync(EmployeeFilter filter)
        //{
        //    var spec = new EmployeeSpecification(x => (!filter.DepartmentId.HasValue || x.EmployeeState.JobFunction.Section.DepartmentId == filter.DepartmentId)
        //     && (string.IsNullOrEmpty(filter.EmployeeName) || (x.FirstName.Contains(filter.EmployeeName) || x.FirstNameThai.Contains(filter.EmployeeName)))
        //     && (string.IsNullOrEmpty(filter.EmployeeId) || x.EmployeeId == filter.EmployeeId)
        //     && (string.IsNullOrEmpty(filter.EmployeeGroup) || x.EmployeeType == filter.EmployeeGroup)
        //     && (!filter.SectionId.HasValue || x.EmployeeState.JobFunction.SectionId == filter.SectionId)
        //     && (!filter.FunctionId.HasValue || x.EmployeeState.JobFunctionId == filter.FunctionId)
        //     && (!filter.ShiftId.HasValue || x.EmployeeState.ShiftId == filter.ShiftId)
        //     && (!filter.LevelId.HasValue || x.EmployeeState.LevelId == filter.LevelId)
        //     && (!filter.PositionId.HasValue || x.EmployeeState.PositionId == filter.PositionId)
        //     && (!filter.AvailableFlag.HasValue || x.AvailableFlag == filter.AvailableFlag));

        //    var employees = await _employeeRepository.GetAsync(spec);
        //    return _mapper.Map<List<Employee>, List<EmployeeModel>>(employees);
        //}

    }

}

using HI.DevOps.DomainCore.Models.TimeSheet;

namespace HI.DevOps.Application.BussinessManagerInterface
{
    public interface ITimeSheetBM
    {
        TimeSheetViewModel Initialize(TimeSheetViewModel timeSheetViewModel);
    }
}
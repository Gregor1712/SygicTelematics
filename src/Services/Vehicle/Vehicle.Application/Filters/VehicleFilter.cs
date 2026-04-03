using Vehicle.Application.Filters.Conditions;

namespace Vehicle.Application.Filters;

public class VehicleFilter: FilterBase
{
    public StringCondition? Vin { get; set; } = new(nameof(Vin));
    public StringCondition? Model { get; set; } = new(nameof(Model));
    public StringCondition? Manufacturer { get; set; } = new(nameof(Manufacturer));
}

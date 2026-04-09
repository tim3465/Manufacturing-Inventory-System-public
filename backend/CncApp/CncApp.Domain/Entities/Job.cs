using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class Job : AuditableEntityBase
{
    // Private constructor for EF Core
    // Sets backing fields directly to avoid validation during materialization
    private Job()
    {
        Shifts = new List<Shift>();
    }

    /// <summary>
    /// Creates a new Job instance with validated invariants.
    /// </summary>
    /// <exception cref="DomainException">Thrown when invariants are violated.</exception>
    public Job(
        int orderId,
        int? stockLotId,
        int machineId,
        int partAmountPlanned,
        int barAmountPlanned,
        TimeSpan barCycleTime,
        int? estimatedPartsPerBar,
        DateOnly dueDate)
    {
        OrderId = orderId;
        StockLotId = stockLotId;
        MachineId = machineId;
        PartAmountPlanned = partAmountPlanned;
        BarAmountPlanned = barAmountPlanned;
        BarCycleTime = barCycleTime;
        EstimatedPartsPerBar = estimatedPartsPerBar;
        DueDate = dueDate;
        Shifts = new List<Shift>();
    }

    private int _orderId;

    public int OrderId
    {
        get => _orderId;
        set
        {
            if (value <= 0)
            {
                throw new DomainException("OrderId must be greater than zero.");
            }

            _orderId = value;
        }
    }

    private int? _stockLotId;

    public int? StockLotId
    {
        get => _stockLotId;
        set
        {
            if (value.HasValue && value.Value <= 0)
            {
                throw new DomainException("StockLotId must be greater than zero.");
            }

            _stockLotId = value;
        }
    }

    private int _machineId;

    public int MachineId
    {
        get => _machineId;
        set
        {
            if (value <= 0)
            {
                throw new DomainException("MachineId must be greater than zero.");
            }

            _machineId = value;
        }
    }

    private int _partAmountPlanned;

    public int PartAmountPlanned
    {
        get => _partAmountPlanned;
        set
        {
            if (value < 0)
            {
                throw new DomainException("PartAmountPlanned must be non-negative.");
            }

            _partAmountPlanned = value;
        }
    }

    private int _barAmountPlanned;

    public int BarAmountPlanned
    {
        get => _barAmountPlanned;
        set
        {
            if (value < 0)
            {
                throw new DomainException("BarAmountPlanned must be non-negative.");
            }

            _barAmountPlanned = value;
        }
    }

    private TimeSpan _barCycleTime;

    public TimeSpan BarCycleTime
    {
        get => _barCycleTime;
        set
        {
            if (value < TimeSpan.Zero)
            {
                throw new DomainException("BarCycleTime must be non-negative.");
            }

            _barCycleTime = value;
        }
    }

    private int _barsInJob;

    public int BarsInJob
    {
        get => _barsInJob;
        set
        {
            if (value < 0)
            {
                throw new DomainException("BarsInJob must be non-negative.");
            }

            _barsInJob = value;
        }
    }

    private int? _estimatedPartsPerBar;

    public int? EstimatedPartsPerBar
    {
        get => _estimatedPartsPerBar;
        set
        {
            if (value.HasValue && value.Value < 0)
            {
                throw new DomainException("EstimatedPartsPerBar must be non-negative when provided.");
            }

            _estimatedPartsPerBar = value;
        }
    }

    private DateOnly _dueDate;

    public DateOnly DueDate
    {
        get => _dueDate;
        set
        {
            if (value == default)
            {
                throw new DomainException("DueDate must not be the default value.");
            }

            _dueDate = value;
        }
    }

    private DateTimeOffset? _startedDateTime;

    public DateTimeOffset? StartedDateTime
    {
        get => _startedDateTime;
        private set => _startedDateTime = value;
    }

    private DateTimeOffset? _endedDateTime;

    public DateTimeOffset? EndedDateTime
    {
        get => _endedDateTime;
        private set => _endedDateTime = value;
    }

    public Order Order { get; set; } = null!;

    public StockLot? StockLot { get; set; }

    public Machine Machine { get; set; } = null!;

    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    /// <summary>
    /// Starts the job by recording the start time and adding bars to the job.
    /// </summary>
    /// <param name="barsToAdd">The number of bars being pulled from inventory into this job.</param>
    /// <exception cref="DomainException">Thrown when the job is already started or barsToAdd is not positive.</exception>
    public void Start(int barsToAdd)
    {
        if (StartedDateTime.HasValue)
        {
            throw new DomainException("Job has already been started.");
        }

        if (barsToAdd <= 0)
        {
            throw new DomainException("BarsToAdd must be greater than zero.");
        }

        StartedDateTime = DateTimeOffset.UtcNow;
        BarsInJob += barsToAdd;
    }

    /// <summary>
    /// Ends the job by recording the end time.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the job is already ended.</exception>
    public void End()
    {
        if (EndedDateTime.HasValue)
        {
            throw new DomainException("Job has already been ended.");
        }
                EndedDateTime = DateTimeOffset.UtcNow;
    }


    /// Closes the job by recording the end time.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the job has not been started or has already been closed.</exception>
    public void Close()
    {
        if (!StartedDateTime.HasValue)
            throw new DomainException("Job cannot be closed because it has not been started.");
        if (EndedDateTime.HasValue)
            throw new DomainException("Job has already been closed.");
        EndedDateTime = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Inactivates the job (soft-delete).
    /// Prevents double-inactivation by throwing a DomainException if already inactivated.
    /// </summary>
    /// <param name="inactivatedByUserId">The ID of the user performing the inactivation (optional).</param>
    /// <exception cref="DomainException">Thrown when the job is already inactivated.</exception>
    public void Inactivate(int? inactivatedByUserId = null)
    {
        if (InactivatedDateTime.HasValue)
        {
            throw new DomainException("Job is already inactivated and cannot be inactivated again.");
        }

        InactivatedDateTime = DateTimeOffset.UtcNow;
        InactivatedByUserId = inactivatedByUserId;
    }
}

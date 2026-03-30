using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class Shift : AuditableEntityBase
{
    // Private constructor for EF Core
    private Shift()
    {
        Job = null!;
        Operator = null!;
    }

    /// <summary>
    /// Creates a new Shift with validated invariants.
    /// </summary>
    public Shift(
        int jobId,
        int operatorId,
        int barsConsumed,
        DateTime startTime,
        int partsMade = 0,
        int scrap = 0,
        int? partsPerBar = null,
        DateTime? stopTime = null,
        TimeSpan? downtime = null)
    {
        JobId = jobId;
        OperatorId = operatorId;
        BarsConsumed = barsConsumed;
        StartTime = startTime;
        PartsMade = partsMade;
        Scrap = scrap;
        PartsPerBar = partsPerBar;
        StopTime = stopTime;
        Downtime = downtime;
        Job = null!;
        Operator = null!;
    }

    private int _jobId;

    public int JobId
    {
        get => _jobId;
        set
        {
            if (value <= 0)
            {
                throw new DomainException("JobId must be greater than zero.");
            }

            _jobId = value;
        }
    }

    private int _operatorId;

    public int OperatorId
    {
        get => _operatorId;
        set
        {
            if (value <= 0)
            {
                throw new DomainException("OperatorId must be greater than zero.");
            }

            _operatorId = value;
        }
    }

    private int _partsMade;

    public int PartsMade
    {
        get => _partsMade;
        set
        {
            if (value < 0)
            {
                throw new DomainException("PartsMade must be non-negative.");
            }

            _partsMade = value;
        }
    }

    private int _scrap;

    public int Scrap
    {
        get => _scrap;
        set
        {
            if (value < 0)
            {
                throw new DomainException("Scrap must be non-negative.");
            }

            _scrap = value;
        }
    }

    private int _barsConsumed;

    public int BarsConsumed
    {
        get => _barsConsumed;
        set
        {
            if (value < 0)
            {
                throw new DomainException("BarsConsumed must be non-negative.");
            }

            _barsConsumed = value;
        }
    }

    private int? _partsPerBar;

    public int? PartsPerBar
    {
        get => _partsPerBar;
        set
        {
            if (value.HasValue && value.Value < 0)
            {
                throw new DomainException("PartsPerBar must be non-negative when provided.");
            }

            _partsPerBar = value;
        }
    }

    private DateTime _startTime;

    public DateTime StartTime
    {
        get => _startTime;
        set
        {
            if (value == default)
            {
                throw new DomainException("StartTime must be provided.");
            }

            _startTime = value;
        }
    }

    public DateTime? StopTime { get; set; }

    public TimeSpan? Downtime { get; set; }

    public Job Job { get; set; } = null!;

    public User Operator { get; set; } = null!;

    public ICollection<ShiftIssueLog> ShiftIssueLogs { get; set; } = new List<ShiftIssueLog>();

    /// <summary>
    /// Inactivates the shift (soft-delete).
    /// Prevents double-inactivation.
    /// </summary>
    public void Inactivate(int? inactivatedByUserId = null)
    {
        if (InactivatedDateTime.HasValue)
        {
            throw new DomainException("Shift is already inactivated and cannot be inactivated again.");
        }

        InactivatedDateTime = DateTimeOffset.UtcNow;
        InactivatedByUserId = inactivatedByUserId;
    }
}

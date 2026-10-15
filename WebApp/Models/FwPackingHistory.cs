using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    /// <summary>
    /// Tracks packing history for bill items
    /// Records: When items are packed, by whom, when packed, quantity packed, etc.
    /// </summary>
    [Table("FW_Packing_History", Schema = "dbo")]
    public class FwPackingHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PackingHistoryId { get; set; }

        [Required]
        public long BillId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int GodownId { get; set; }

        /// <summary>
        /// Quantity packed for this item
        /// </summary>
        [Required]
        public int QuantityPacked { get; set; }

        /// <summary>
        /// Allocated quantity for this item (from allocation process)
        /// </summary>
        public int? AllocatedQty { get; set; }

        /// <summary>
        /// Computed remaining quantity (AllocatedQty - QuantityPacked)
        /// Stored as a computed column in the database
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? RemainingQty { get; private set; }

        /// <summary>
        /// Type of dispatch requested/used (e.g. "Regular", "Express")
        /// </summary>
        [StringLength(20)]
        public string? DispatchType { get; set; }

        /// <summary>
        /// Number of cases/units packed
        /// </summary>
        public int? NumberOfCasesPacked { get; set; }

        /// <summary>
        /// User ID of person who packed the items
        /// </summary>
        [Required]
        public int PackedByUserId { get; set; }

        /// <summary>
        /// When the items were packed
        /// </summary>
        [Required]
        public DateTime PackedDateTime { get; set; }

        /// <summary>
        /// Optional notes/remarks about the packing
        /// </summary>
        [StringLength(500)]
        public string? PackingNotes { get; set; }

        /// <summary>
        /// Packing status: P (Partial), C (Complete)
        /// </summary>
        [StringLength(1)]
        public string? PackingStatus { get; set; }

        /// <summary>
        /// Record creation date
        /// </summary>
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Last modification date
        /// </summary>
        public DateTime? ModifiedDateTime { get; set; }

        /// <summary>
        /// User who last modified
        /// </summary>
        public int? ModifiedByUserId { get; set; }

        // Foreign key relationships
        [ForeignKey("BillId")]
        public virtual FwBill Bill { get; set; }

        [ForeignKey("ItemId")]
        public virtual FwMasterItem Item { get; set; }

        [ForeignKey("GodownId")]
        public virtual FwMasterGodown Godown { get; set; }

        [ForeignKey("PackedByUserId")]
        public virtual FwUserDetail PackedByUser { get; set; }

        [ForeignKey("ModifiedByUserId")]
        public virtual FwUserDetail ModifiedByUser { get; set; }
    }
}

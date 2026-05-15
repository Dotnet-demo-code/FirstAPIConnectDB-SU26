using System;
using System.Collections.Generic;

namespace FirstAPIConnectDB.Models;

public partial class BorrowRecord
{
    public int BorrowId { get; set; }

    public int? UserId { get; set; }

    public int? BookId { get; set; }

    public DateOnly BorrowDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public virtual Book? Book { get; set; }

    public virtual User? User { get; set; }
}

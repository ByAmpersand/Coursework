﻿using System;
using System.Collections.Generic;
using BookstoreBackend.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreBackend.Entities;

public partial class Address
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string Region { get; set; } = null!;

    public string City { get; set; } = null!;

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string PostalCode { get; set; } = null!;
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
}

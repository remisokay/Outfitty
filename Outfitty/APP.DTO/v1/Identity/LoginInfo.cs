﻿using System.ComponentModel.DataAnnotations;

namespace APP.DTO.v1.Identity;

public class LoginInfo
{
    [MaxLength(128)]
    public string Email { get; set; } = default!;
    [MaxLength(128)]
    public string Password { get; set; } = default!;
    
}
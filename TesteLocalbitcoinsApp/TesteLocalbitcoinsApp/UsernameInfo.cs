//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TesteLocalbitcoinsApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class UsernameInfo
    {
        public int UsernameKey { get; set; }
        public string Email { get; set; }
        public bool Verified { get; set; }
        public Nullable<System.DateTime> Verification_date { get; set; }
        public Nullable<decimal> Total_Bought { get; set; }
        public Nullable<decimal> Total_Sold { get; set; }
        public int Number_complete_trades { get; set; }
        public Nullable<System.DateTime> AccountCreatedAt { get; set; }
        public string RealName { get; set; }
        public Nullable<bool> FeedBackExists { get; set; }
        public string myFeedBackMessage { get; set; }
        public string feedbacktype { get; set; }
    
        public virtual Username Username { get; set; }
    }
}

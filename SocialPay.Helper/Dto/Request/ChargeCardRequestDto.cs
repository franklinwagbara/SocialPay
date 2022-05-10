public class ChargeCardRequestDto
{
    public string pan { get; set; }
    public string cvv { get; set; }
    public int expiryMonth { get; set; }
    public int expiryYear { get; set; }
    public int currency { get; set; }
    public string pin { get; set; }
    public string Email { get; set; }
}
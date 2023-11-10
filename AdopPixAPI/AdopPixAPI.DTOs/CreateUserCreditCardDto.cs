namespace AdopPixAPI.DTOs
{
    public class CreateUserCreditCardDto
    {
        public string CardNumber { get; set; }
        public string NameOnCard { get; set; }
        public string ExpireDate { get; set; }
    }
}

using Amazon.DynamoDBv2.DataModel;

namespace Lab3.Models
{

    [DynamoDBTable("Movies")]
    public class Movie
    {
        [DynamoDBHashKey("id")]
        public int Id { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("title")]
        public String Title { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("releaseYear")]
        public int ReleaseYear { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("rating")]
        public double Rating { get; set; }

        [DynamoDBProperty("rateCount")]
        public int RateCount { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("genre")]
        public String Genre { get; set; }

        [DynamoDBProperty("director")]
        public String Director { get; set; }

        [DynamoDBProperty("comments")]
        public List<String> Comments { get; set; }

        [DynamoDBProperty("userId")]
        public String UserId { get; set; }


        [DynamoDBProperty("imgUrl")]
        public String ImgUrl { get; set; }


    }

}

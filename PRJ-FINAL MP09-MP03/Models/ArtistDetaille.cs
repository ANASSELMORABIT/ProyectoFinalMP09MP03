using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRJ_FINAL_MP09_MP03.Models
{
    public class ArtistDetaille
    {

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Annotatable
        {
            public string _type { get; set; }
            public string api_path { get; set; }
            public object context { get; set; }
            public int id { get; set; }
            public string image_url { get; set; }
            public string link_title { get; set; }
            public string title { get; set; }
            public string type { get; set; }
            public string url { get; set; }
        }

        public class Annotation
        {
            public string _type { get; set; }
            public string api_path { get; set; }
            public bool being_created { get; set; }
            public Body body { get; set; }
            public int comment_count { get; set; }
            public bool community { get; set; }
            public int created_at { get; set; }
            public object custom_preview { get; set; }
            public bool deleted { get; set; }
            public string embed_content { get; set; }
            public bool has_voters { get; set; }
            public int id { get; set; }
            public bool needs_exegesis { get; set; }
            public bool pinned { get; set; }
            public int proposed_edit_count { get; set; }
            public int pyongs_count { get; set; }
            public int referent_id { get; set; }
            public string share_url { get; set; }
            public object source { get; set; }
            public string state { get; set; }
            public string twitter_share_message { get; set; }
            public string url { get; set; }
            public bool verified { get; set; }
            public int votes_total { get; set; }
            public CurrentUserMetadata current_user_metadata { get; set; }
            public object accepted_by { get; set; }
            public List<Author> authors { get; set; }
            public List<object> cosigned_by { get; set; }
            public CreatedBy created_by { get; set; }
            public object rejection_comment { get; set; }
            public object top_comment { get; set; }
            public object verified_by { get; set; }
        }

        public class Artist
        {
            public string _type { get; set; }
            public List<string> alternate_names { get; set; }
            public string api_path { get; set; }
            public Description description { get; set; }
            public string description_preview { get; set; }
            public string facebook_name { get; set; }
            public string header_image_url { get; set; }
            public int id { get; set; }
            public string image_url { get; set; }
            public string index_character { get; set; }
            public string instagram_name { get; set; }
            public bool is_meme_verified { get; set; }
            public bool is_verified { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public TrackingPaths tracking_paths { get; set; }
            public bool translation_artist { get; set; }
            public string twitter_name { get; set; }
            public string url { get; set; }
            public CurrentUserMetadata current_user_metadata { get; set; }
            public int followers_count { get; set; }
            public int iq { get; set; }
            public DescriptionAnnotation description_annotation { get; set; }
            public User user { get; set; }
        }

        public class Author
        {
            public string _type { get; set; }
            public object  attribution { get; set; }
            public object pinned_role { get; set; }
            public User user { get; set; }
        }

        public class Avatar
        {
            public Tiny tiny { get; set; }
            public Thumb thumb { get; set; }
            public Small small { get; set; }
            public Medium medium { get; set; }
        }

        public class Body
        {
            public string html { get; set; }
        }

        public class BoundingBox
        {
            public int width { get; set; }
            public int height { get; set; }
        }

        public class CreatedBy
        {
            public string _type { get; set; }
            public string about_me_summary { get; set; }
            public string api_path { get; set; }
            public Avatar avatar { get; set; }
            public string header_image_url { get; set; }
            public string human_readable_role_for_display { get; set; }
            public int id { get; set; }
            public int iq { get; set; }
            public bool is_meme_verified { get; set; }
            public bool is_verified { get; set; }
            public string login { get; set; }
            public string name { get; set; }
            public string role_for_display { get; set; }
            public string url { get; set; }
            public CurrentUserMetadata current_user_metadata { get; set; }
        }

        public class CurrentUserMetadata
        {
            public List<string> permissions { get; set; }
            public List<string> excluded_permissions { get; set; }
            public Interactions interactions { get; set; }
            public Relationships relationships { get; set; }
            public IqByAction iq_by_action { get; set; }
        }

        public class Description
        {
            public string html { get; set; }
        }

        public class DescriptionAnnotation
        {
            public string _type { get; set; }
            public int annotator_id { get; set; }
            public string annotator_login { get; set; }
            public string api_path { get; set; }
            public string classification { get; set; }
            public string fragment { get; set; }
            public int id { get; set; }
            public string ios_app_url { get; set; }
            public bool is_description { get; set; }
            public bool is_image { get; set; }
            public string path { get; set; }
            public Range range { get; set; }
            public object song_id { get; set; }
            public string url { get; set; }
            public List<object> verified_annotator_ids { get; set; }
            public CurrentUserMetadata current_user_metadata { get; set; }
            public TrackingPaths tracking_paths { get; set; }
            public string twitter_share_message { get; set; }
            public Annotatable annotatable { get; set; }
            public List<Annotation> annotations { get; set; }
        }

        public class Interactions
        {
            public bool following { get; set; }
            public bool cosign { get; set; }
            public bool pyong { get; set; }
            public object vote { get; set; }
        }

        public class IqByAction
        {
        }

        public class Medium
        {
            public string url { get; set; }
            public BoundingBox bounding_box { get; set; }
        }

        public class Range
        {
            public string content { get; set; }
        }

        public class Relationships
        {
        }

        public class Root
        {
            public Artist artist { get; set; }
        }

        public class Small
        {
            public string url { get; set; }
            public BoundingBox bounding_box { get; set; }
        }

        public class Thumb
        {
            public string url { get; set; }
            public BoundingBox bounding_box { get; set; }
        }

        public class Tiny
        {
            public string url { get; set; }
            public BoundingBox bounding_box { get; set; }
        }

        public class TrackingPaths
        {
            public string aggregate { get; set; }
            public string concurrent { get; set; }
        }

        public class User
        {
            public string _type { get; set; }
            public string about_me_summary { get; set; }
            public string api_path { get; set; }
            public Avatar avatar { get; set; }
            public string header_image_url { get; set; }
            public string human_readable_role_for_display { get; set; }
            public int id { get; set; }
            public int iq { get; set; }
            public bool is_meme_verified { get; set; }
            public bool is_verified { get; set; }
            public string login { get; set; }
            public string name { get; set; }
            public string role_for_display { get; set; }
            public string url { get; set; }
            public CurrentUserMetadata current_user_metadata { get; set; }
        }


    }

}
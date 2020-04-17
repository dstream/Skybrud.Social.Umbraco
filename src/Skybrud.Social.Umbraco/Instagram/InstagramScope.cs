using System;

namespace Skybrud.Social.Umbraco.Instagram
{

    [Flags]
    public enum InstagramScope {

        /// <summary>
        /// Grants your app permission to read the app user's profile. (e.g. following/followed-by lists, photos, etc.) (granted by default).
        /// </summary>
        user_profile = 0,

        /// <summary>
        /// Grants your app permission to read the app user's IG Media objects.
        /// Allowed Usage:
        /// - Creating physical or digital books from the app user's photos, including exporting photos for printing
        /// - Displaying the app users photos to other app users within the app (e.g, for Dating or Social Network Applications)
        /// - Editing or creating new photos or videos based on the app user's existing photos and videos (Photo/Video Editing Apps)
        /// - Displaying the app user's photos and videos in an external device such as a TV or digital photo frame
        /// </summary>
        user_media = 1,            
    }

    [Flags]
    public enum InstagramGraphScope
    {

        /// <summary>
        /// The instagram_basic permission allows your app to read an Instagram Account profile's info and media.
        /// Allowed Usage: Get basic metadata of an Instagram Business Account like username and ID.
        /// </summary>
        instagram_basic = 0,

        /// <summary>
        /// The instagram_content_publish permission allows your app to create organic feed photo and video posts on behalf of a business user.
        /// Allowed Usage:
        /// - Managing organic content creation process for Instagram (i.e. post photos, videos to main feed) on behalf of a business.        
        /// </summary>
        //instagram_content_publish = 1,

        /// <summary>
        /// The instagram_manage_comments permission allows your app to create, delete, and hide comments on behalf of the Instagram Account linked to the Page. Your app can also read and respond to public media and comments that a business has been photo tagged or @mentioned in.
        /// Allowed Usage:
        /// - Read, update, and delete comments of Instagram Business Accounts.       
        /// - Read media objects, such as stories, of Instagram Business Accounts.
        /// </summary>
        //instagram_manage_comments = 2,

        /// <summary>
        /// The instagram_manage_insights permission allows your app to get access to insights for the Instagram account linked to the Facebook Page. You app can also discover and read the profile info and media of other business profiles.
        /// Allowed Usage:
        /// - Get metadata of an Instagram Business Account.
        /// - Get data insights of an Instagram Business Account.
        /// - Get story insights of an Instagram Business Account.
        /// </summary>
        //instagram_manage_insights = 3,

        /// <summary>
        /// The pages_show_list permission allows your app to show the list of the Pages that a person manages. Does not require App Review.
        /// Allowed Usage:
        /// - Provide API access to accounts for showing the list of the Pages that a person manages.
        /// </summary>
        pages_show_list = 4

    }
}
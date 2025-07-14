using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApiTokensTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColorThemes",
                columns: table => new
                {
                    color_theme_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    css_class = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    color_variables = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    is_default = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorThemes", x => x.color_theme_id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    language_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    short_code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    icon_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    is_default = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.language_id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    tag_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    usage_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.tag_id);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    topic_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    topic_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.topic_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "ApiTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailPasswordAuths",
                columns: table => new
                {
                    email_password_auth_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    refresh_token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    refresh_token_expires_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    refresh_token_revoked_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailPasswordAuths", x => x.email_password_auth_id);
                    table.ForeignKey(
                        name: "FK_EmailPasswordAuths_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoogleAuths",
                columns: table => new
                {
                    google_auth_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    google_id = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    picture_url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    refresh_token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    refresh_token_expires_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    refresh_token_revoked_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoogleAuths", x => x.google_auth_id);
                    table.ForeignKey(
                        name: "FK_GoogleAuths_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    template_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    author_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    topic_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    access_type = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_archived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_published = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    version = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    base_template_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    previous_version_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.template_id);
                    table.ForeignKey(
                        name: "FK_Templates_Topics_topic_id",
                        column: x => x.topic_id,
                        principalTable: "Topics",
                        principalColumn: "topic_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Templates_Users_author_id",
                        column: x => x.author_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserContacts",
                columns: table => new
                {
                    user_contact_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    contact_type = table.Column<int>(type: "int", nullable: false),
                    contact_value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    is_primary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserContacts", x => x.user_contact_id);
                    table.ForeignKey(
                        name: "FK_UserContacts_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    user_settings_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    color_theme_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    language_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.user_settings_id);
                    table.ForeignKey(
                        name: "FK_UserSettings_ColorThemes_color_theme_id",
                        column: x => x.color_theme_id,
                        principalTable: "ColorThemes",
                        principalColumn: "color_theme_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSettings_Languages_language_id",
                        column: x => x.language_id,
                        principalTable: "Languages",
                        principalColumn: "language_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    comment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    template_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK_Comments_Templates_template_id",
                        column: x => x.template_id,
                        principalTable: "Templates",
                        principalColumn: "template_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    form_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    template_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    answers_data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    submitted_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    template_version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.form_id);
                    table.ForeignKey(
                        name: "FK_Forms_Templates_template_id",
                        column: x => x.template_id,
                        principalTable: "Templates",
                        principalColumn: "template_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Forms_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormSubscribes",
                columns: table => new
                {
                    form_subscribe_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    template_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSubscribes", x => x.form_subscribe_id);
                    table.ForeignKey(
                        name: "FK_FormSubscribes_Templates_template_id",
                        column: x => x.template_id,
                        principalTable: "Templates",
                        principalColumn: "template_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormSubscribes_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    like_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    template_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.like_id);
                    table.ForeignKey(
                        name: "FK_Likes_Templates_template_id",
                        column: x => x.template_id,
                        principalTable: "Templates",
                        principalColumn: "template_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    question_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    template_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order = table.Column<int>(type: "int", nullable: false),
                    show_in_results = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_required = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.question_id);
                    table.ForeignKey(
                        name: "FK_Questions_Templates_template_id",
                        column: x => x.template_id,
                        principalTable: "Templates",
                        principalColumn: "template_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateAllowedUsers",
                columns: table => new
                {
                    template_allowed_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    template_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateAllowedUsers", x => x.template_allowed_user_id);
                    table.ForeignKey(
                        name: "FK_TemplateAllowedUsers_Templates_template_id",
                        column: x => x.template_id,
                        principalTable: "Templates",
                        principalColumn: "template_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateAllowedUsers_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateTags",
                columns: table => new
                {
                    template_tag_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    template_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tag_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateTags", x => x.template_tag_id);
                    table.ForeignKey(
                        name: "FK_TemplateTags_Tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "Tags",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateTags_Templates_template_id",
                        column: x => x.template_id,
                        principalTable: "Templates",
                        principalColumn: "template_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiTokens_TokenHash",
                table: "ApiTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiTokens_UserId",
                table: "ApiTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiTokens_UserId_IsActive",
                table: "ApiTokens",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "idx_ColorThemes_CssClass",
                table: "ColorThemes",
                column: "css_class",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_ColorThemes_Name",
                table: "ColorThemes",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_comments_template",
                table: "Comments",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_user_id",
                table: "Comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_email_password_auths_email",
                table: "EmailPasswordAuths",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_email_password_auths_refresh_token",
                table: "EmailPasswordAuths",
                column: "refresh_token");

            migrationBuilder.CreateIndex(
                name: "idx_email_password_auths_user",
                table: "EmailPasswordAuths",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_forms_template",
                table: "Forms",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "idx_forms_template_user",
                table: "Forms",
                columns: new[] { "template_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_forms_user",
                table: "Forms",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_form_subscribes_template",
                table: "FormSubscribes",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "idx_form_subscribes_user",
                table: "FormSubscribes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_form_subscribes_user_template",
                table: "FormSubscribes",
                columns: new[] { "user_id", "template_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_google_auths_email",
                table: "GoogleAuths",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_google_auths_google_id",
                table: "GoogleAuths",
                column: "google_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_google_auths_refresh_token",
                table: "GoogleAuths",
                column: "refresh_token");

            migrationBuilder.CreateIndex(
                name: "idx_google_auths_user",
                table: "GoogleAuths",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_Languages_Code",
                table: "Languages",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_Languages_Name",
                table: "Languages",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_likes_template",
                table: "Likes",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "idx_likes_template_user",
                table: "Likes",
                columns: new[] { "template_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Likes_user_id",
                table: "Likes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_questions_template",
                table: "Questions",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "idx_questions_template_order",
                table: "Questions",
                columns: new[] { "template_id", "order" });

            migrationBuilder.CreateIndex(
                name: "idx_tags_name",
                table: "Tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_template_allowed_users_template",
                table: "TemplateAllowedUsers",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "idx_template_allowed_users_template_user",
                table: "TemplateAllowedUsers",
                columns: new[] { "template_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateAllowedUsers_user_id",
                table: "TemplateAllowedUsers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_templates_author",
                table: "Templates",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "idx_templates_base_template",
                table: "Templates",
                column: "base_template_id");

            migrationBuilder.CreateIndex(
                name: "idx_templates_base_version",
                table: "Templates",
                columns: new[] { "base_template_id", "version" },
                unique: true,
                filter: "[base_template_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_topic_id",
                table: "Templates",
                column: "topic_id");

            migrationBuilder.CreateIndex(
                name: "idx_template_tags_tag",
                table: "TemplateTags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "idx_template_tags_template_tag",
                table: "TemplateTags",
                columns: new[] { "template_id", "tag_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_topic_name",
                table: "Topics",
                column: "topic_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_user_contacts_user",
                table: "UserContacts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_user_contacts_user_type_primary",
                table: "UserContacts",
                columns: new[] { "user_id", "contact_type", "is_primary" });

            migrationBuilder.CreateIndex(
                name: "idx_users_username",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_UserSettings_user_Id",
                table: "UserSettings",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_color_theme_id",
                table: "UserSettings",
                column: "color_theme_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_language_id",
                table: "UserSettings",
                column: "language_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiTokens");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "EmailPasswordAuths");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropTable(
                name: "FormSubscribes");

            migrationBuilder.DropTable(
                name: "GoogleAuths");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "TemplateAllowedUsers");

            migrationBuilder.DropTable(
                name: "TemplateTags");

            migrationBuilder.DropTable(
                name: "UserContacts");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "ColorThemes");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

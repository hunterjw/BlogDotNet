CREATE OR REPLACE VIEW blogdotnet.view_ranked_blog_post AS
	SELECT ROW_NUMBER() OVER () AS rank, * 
	FROM (
		SELECT *
		FROM blogdotnet.blog_post
		ORDER BY pinned DESC, pub_date DESC
	) inner_query
-- Insert admin user
INSERT INTO users (username, email, password_hash, display_name, role, is_active)
VALUES 
    ('admin', 'admin@blog.local', '$2b$12$abc123def456...', 'Administrator', 3, true),
    ('moderator', 'mod@blog.local', '$2b$12$abc123def456...', 'Moderator', 2, true),
    ('user1', 'user1@blog.local', '$2b$12$abc123def456...', 'John Doe', 1, true),
    ('user2', 'user2@blog.local', '$2b$12$abc123def456...', 'Jane Smith', 1, true)
ON CONFLICT (username) DO NOTHING;

-- Insert tags
INSERT INTO tags (name, slug, description, color, is_active)
VALUES 
    ('C#', 'csharp', 'C# programming language', '#239120', true),
    ('ASP.NET Core', 'aspnet-core', 'ASP.NET Core framework', '#512BD4', true),
    ('PostgreSQL', 'postgresql', 'PostgreSQL database', '#336791', true),
    ('Docker', 'docker', 'Docker containerization', '#2496ED', true),
    ('Database', 'database', 'Database design and optimization', '#FF9900', true)
ON CONFLICT (slug) DO NOTHING;

-- Insert sample posts
INSERT INTO posts (user_id, title, content, summary, is_published, published_at)
VALUES 
    (3, 'Getting Started with C# and PostgreSQL', 
     'In this tutorial, we will explore how to connect to PostgreSQL using C#...',
     'Learn C# and PostgreSQL integration', true, CURRENT_TIMESTAMP),
    (3, 'Docker Best Practices',
     'Docker is a powerful containerization platform. Here are the best practices...',
     'Essential Docker practices for production', true, CURRENT_TIMESTAMP),
    (4, 'ASP.NET Core Performance Tips',
     'Performance is critical for web applications. Let''s discuss some optimization techniques...',
     'Optimize your ASP.NET Core applications', true, CURRENT_TIMESTAMP)
ON CONFLICT DO NOTHING;

-- Insert post tags
INSERT INTO post_tags (post_id, tag_id)
VALUES 
    (1, 1),  -- Post 1: C#
    (1, 3),  -- Post 1: PostgreSQL
    (2, 4),  -- Post 2: Docker
    (3, 2),  -- Post 3: ASP.NET Core
    (3, 1)   -- Post 3: C#
ON CONFLICT DO NOTHING;

-- Update tags count
UPDATE tags SET posts_count = (
    SELECT COUNT(DISTINCT post_id) FROM post_tags WHERE tag_id = tags.id
);

-- User subscriptions to tags
INSERT INTO user_tags (user_id, tag_id)
VALUES 
    (3, 1),  -- user1 subscribed to C#
    (3, 2),  -- user1 subscribed to ASP.NET Core
    (3, 3),  -- user1 subscribed to PostgreSQL
    (4, 2),  -- user2 subscribed to ASP.NET Core
    (4, 4)   -- user2 subscribed to Docker
ON CONFLICT DO NOTHING;


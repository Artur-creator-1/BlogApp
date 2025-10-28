-- users table
CREATE TABLE IF NOT EXISTS users (
    id              BIGSERIAL PRIMARY KEY,
    username        VARCHAR(64)  NOT NULL UNIQUE,
    email           VARCHAR(255) NOT NULL UNIQUE,
    password_hash   VARCHAR(255) NOT NULL,
    display_name    VARCHAR(128),
    bio             TEXT,
    role            INT          NOT NULL DEFAULT 1,  -- 1=User, 2=Moderator, 3=Admin
    is_active       BOOLEAN      NOT NULL DEFAULT TRUE,
    last_login_at   TIMESTAMP,
    created_at      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP
);

-- tags table
CREATE TABLE IF NOT EXISTS tags (
    id              BIGSERIAL PRIMARY KEY,
    name            VARCHAR(64)  NOT NULL UNIQUE,
    slug            VARCHAR(128) NOT NULL UNIQUE,
    description     TEXT,
    color           VARCHAR(16),
    posts_count     INT          NOT NULL DEFAULT 0,
    is_active       BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP
);

-- posts table
CREATE TABLE IF NOT EXISTS posts (
    id              BIGSERIAL PRIMARY KEY,
    user_id         BIGINT       NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    title           VARCHAR(200) NOT NULL,
    content         TEXT         NOT NULL,
    summary         TEXT,
    image_url       VARCHAR(255),
    view_count      INT          NOT NULL DEFAULT 0,
    likes_count     INT          NOT NULL DEFAULT 0,
    comments_count  INT          NOT NULL DEFAULT 0,
    is_published    BOOLEAN      NOT NULL DEFAULT TRUE,
    published_at    TIMESTAMP,
    is_deleted      BOOLEAN      NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP
);

-- post_tags table (many-to-many)
CREATE TABLE IF NOT EXISTS post_tags (
    post_id         BIGINT NOT NULL REFERENCES posts(id) ON DELETE CASCADE,
    tag_id          BIGINT NOT NULL REFERENCES tags(id) ON DELETE CASCADE,
    created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (post_id, tag_id)
);

-- user_tags table (many-to-many) - для персональной ленты
CREATE TABLE IF NOT EXISTS user_tags (
    user_id         BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    tag_id          BIGINT NOT NULL REFERENCES tags(id) ON DELETE CASCADE,
    created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, tag_id)
);

-- comments table
CREATE TABLE IF NOT EXISTS comments (
    id              BIGSERIAL PRIMARY KEY,
    post_id         BIGINT NOT NULL REFERENCES posts(id) ON DELETE CASCADE,
    user_id         BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    parent_comment_id BIGINT REFERENCES comments(id) ON DELETE SET NULL,
    content         TEXT NOT NULL,
    likes_count     INT NOT NULL DEFAULT 0,
    is_edited       BOOLEAN NOT NULL DEFAULT FALSE,
    is_deleted      BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at      TIMESTAMP
);

-- post_likes table
CREATE TABLE IF NOT EXISTS post_likes (
    post_id         BIGINT NOT NULL REFERENCES posts(id) ON DELETE CASCADE,
    user_id         BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (post_id, user_id)
);

-- comment_likes table
CREATE TABLE IF NOT EXISTS comment_likes (
    comment_id      BIGINT NOT NULL REFERENCES comments(id) ON DELETE CASCADE,
    user_id         BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (comment_id, user_id)
);

-- ============================================================
-- INDEXES FOR OPTIMIZATION
-- ============================================================

-- Posts indexes
CREATE INDEX idx_posts_user_id ON posts(user_id);
CREATE INDEX idx_posts_published ON posts(is_published, published_at DESC);
CREATE INDEX idx_posts_created ON posts(created_at DESC);
CREATE INDEX idx_posts_is_deleted ON posts(is_deleted);

-- Comments indexes
CREATE INDEX idx_comments_post_id ON comments(post_id);
CREATE INDEX idx_comments_user_id ON comments(user_id);
CREATE INDEX idx_comments_parent_id ON comments(parent_comment_id);
CREATE INDEX idx_comments_is_deleted ON comments(is_deleted);

-- Tags indexes
CREATE INDEX idx_tags_slug ON tags(slug);
CREATE INDEX idx_tags_is_active ON tags(is_active);

-- Post likes indexes
CREATE INDEX idx_post_likes_user ON post_likes(user_id);

-- Comment likes indexes
CREATE INDEX idx_comment_likes_user ON comment_likes(user_id);

-- User tags indexes
CREATE INDEX idx_user_tags_user ON user_tags(user_id);
CREATE INDEX idx_user_tags_tag ON user_tags(tag_id);

-- Role values: 1=User, 2=Moderator, 3=Admin
ALTER TABLE users ADD CONSTRAINT check_role_values CHECK (role IN (1, 2, 3));
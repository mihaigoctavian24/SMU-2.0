-- =====================================================
-- MATERIALIZED VIEW REFRESH SCRIPT
-- Created: November 30, 2025
-- Purpose: Refresh analytics materialized views
-- Schedule: Run every 6 hours via cron or scheduler
-- =====================================================

-- =====================================================
-- REFRESH FACULTY STATISTICS
-- =====================================================
REFRESH MATERIALIZED VIEW CONCURRENTLY mv_faculty_statistics;

-- =====================================================
-- LOG REFRESH ACTIVITY
-- =====================================================
DO $$
DECLARE
    refresh_count INT;
BEGIN
    SELECT COUNT(*) INTO refresh_count FROM mv_faculty_statistics;
    
    -- Log to audit
    INSERT INTO audit_logs (
        user_id,
        action,
        entity_type,
        entity_id,
        new_values
    )
    VALUES (
        NULL, -- System action
        'REFRESH_MATERIALIZED_VIEW',
        'mv_faculty_statistics',
        NULL,
        jsonb_build_object(
            'refreshed_at', NOW(),
            'row_count', refresh_count,
            'success', true
        )
    );
    
    RAISE NOTICE 'Successfully refreshed mv_faculty_statistics with % rows at %', refresh_count, NOW();
END;
$$;

-- =====================================================
-- VACUUM ANALYZE (OPTIONAL - for performance)
-- =====================================================
VACUUM ANALYZE mv_faculty_statistics;

-- =====================================================
-- USAGE INSTRUCTIONS
-- =====================================================
-- 
-- To set up automatic refresh, add to crontab (Linux/Mac):
-- 0 */6 * * * psql -d your_database -f /path/to/refresh_materialized_views.sql
--
-- Or use pg_cron extension:
-- SELECT cron.schedule('refresh-analytics', '0 */6 * * *', 
--     'REFRESH MATERIALIZED VIEW CONCURRENTLY mv_faculty_statistics;');
--
-- For Supabase, create a Supabase Function triggered by cron:
-- create function refresh_analytics_views()
-- returns void
-- language plpgsql
-- as $$
-- begin
--   refresh materialized view concurrently mv_faculty_statistics;
-- end;
-- $$;
--
-- Then schedule via Supabase Dashboard > Database > Cron Jobs
-- =====================================================

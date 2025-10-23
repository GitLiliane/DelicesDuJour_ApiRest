--
-- PostgreSQL database dump
--

-- Dumped from database version 17.2 (Debian 17.2-1.pgdg120+1)
-- Dumped by pg_dump version 17.0

-- Started on 2025-10-22 18:39:30

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 5 (class 2615 OID 164872)
-- Name: public; Type: SCHEMA; Schema: -; Owner: postgres
--

-- *not* creating schema, since initdb creates it

DROP SCHEMA IF EXISTS public CASCADE;
CREATE SCHEMA IF NOT EXISTS public;

--
-- TOC entry 5 (class 2615 OID 172329)
-- Name: public; Type: SCHEMA; Schema: -; Owner: postgres
--

-- *not* creating schema, since initdb creates it


ALTER SCHEMA public OWNER TO postgres;

--
-- TOC entry 3454 (class 0 OID 0)
-- Dependencies: 5
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: postgres
--

COMMENT ON SCHEMA public IS '';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 217 (class 1259 OID 172330)
-- Name: avis; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.avis (
    id_recette integer NOT NULL,
    id_utilisateur integer NOT NULL,
    note integer NOT NULL,
    commentaire character varying(500),
    date_commentaire date,
    CONSTRAINT avis_note_check CHECK (((note >= 1) AND (note <= 5)))
);


ALTER TABLE public.avis OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 172336)
-- Name: categories; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.categories (
    id integer NOT NULL,
    nom character varying(50) NOT NULL
);


ALTER TABLE public.categories OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 172339)
-- Name: categories_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.categories_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.categories_id_seq OWNER TO postgres;

--
-- TOC entry 3456 (class 0 OID 0)
-- Dependencies: 219
-- Name: categories_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.categories_id_seq OWNED BY public.categories.id;


--
-- TOC entry 220 (class 1259 OID 172340)
-- Name: categories_recettes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.categories_recettes (
    id_categorie integer NOT NULL,
    id_recette integer NOT NULL
);


ALTER TABLE public.categories_recettes OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 172343)
-- Name: etapes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.etapes (
    id_recette integer NOT NULL,
    numero integer NOT NULL,
    titre character varying(200),
    texte character varying(5000)
);


ALTER TABLE public.etapes OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 172348)
-- Name: ingredients; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ingredients (
    id integer NOT NULL,
    nom character varying(50) NOT NULL
);


ALTER TABLE public.ingredients OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 172351)
-- Name: ingredients_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.ingredients_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.ingredients_id_seq OWNER TO postgres;

--
-- TOC entry 3457 (class 0 OID 0)
-- Dependencies: 223
-- Name: ingredients_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredients_id_seq OWNED BY public.ingredients.id;


--
-- TOC entry 224 (class 1259 OID 172352)
-- Name: ingredients_recettes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ingredients_recettes (
    id_ingredient integer NOT NULL,
    id_recette integer NOT NULL,
    quantite character varying(40)
);


ALTER TABLE public.ingredients_recettes OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 172355)
-- Name: recettes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.recettes (
    id integer NOT NULL,
    nom character varying(100) NOT NULL,
    temps_preparation interval NOT NULL,
    temps_cuisson interval NOT NULL,
    difficulte integer NOT NULL,
    photo character varying(100) DEFAULT 'default.jpg'::character varying,
    id_utilisateur integer,
    CONSTRAINT recettes_difficulte_check CHECK (((difficulte >= 1) AND (difficulte <= 3)))
);


ALTER TABLE public.recettes OWNER TO postgres;

--
-- TOC entry 226 (class 1259 OID 172360)
-- Name: recettes_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.recettes_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.recettes_id_seq OWNER TO postgres;

--
-- TOC entry 3458 (class 0 OID 0)
-- Dependencies: 226
-- Name: recettes_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.recettes_id_seq OWNED BY public.recettes.id;


--
-- TOC entry 227 (class 1259 OID 172361)
-- Name: roles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.roles (
    id integer NOT NULL,
    nom character varying(100) NOT NULL
);


ALTER TABLE public.roles OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 172364)
-- Name: roles_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.roles_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.roles_id_seq OWNER TO postgres;

--
-- TOC entry 3459 (class 0 OID 0)
-- Dependencies: 228
-- Name: roles_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.roles_id_seq OWNED BY public.roles.id;


--
-- TOC entry 229 (class 1259 OID 172365)
-- Name: utilisateurs; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.utilisateurs (
    id integer NOT NULL,
    identifiant character varying(20) NOT NULL,
    email character varying(50) NOT NULL,
    pass_word character varying(250) NOT NULL,
    role_id integer
);


ALTER TABLE public.utilisateurs OWNER TO postgres;

--
-- TOC entry 230 (class 1259 OID 172368)
-- Name: utilisateurs_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.utilisateurs_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.utilisateurs_id_seq OWNER TO postgres;

--
-- TOC entry 3460 (class 0 OID 0)
-- Dependencies: 230
-- Name: utilisateurs_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.utilisateurs_id_seq OWNED BY public.utilisateurs.id;


--
-- TOC entry 3246 (class 2604 OID 172369)
-- Name: categories id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categories ALTER COLUMN id SET DEFAULT nextval('public.categories_id_seq'::regclass);


--
-- TOC entry 3247 (class 2604 OID 172370)
-- Name: ingredients id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients ALTER COLUMN id SET DEFAULT nextval('public.ingredients_id_seq'::regclass);


--
-- TOC entry 3248 (class 2604 OID 172371)
-- Name: recettes id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recettes ALTER COLUMN id SET DEFAULT nextval('public.recettes_id_seq'::regclass);


--
-- TOC entry 3250 (class 2604 OID 172372)
-- Name: roles id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles ALTER COLUMN id SET DEFAULT nextval('public.roles_id_seq'::regclass);


--
-- TOC entry 3251 (class 2604 OID 172373)
-- Name: utilisateurs id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.utilisateurs ALTER COLUMN id SET DEFAULT nextval('public.utilisateurs_id_seq'::regclass);


--
-- TOC entry 3434 (class 0 OID 172330)
-- Dependencies: 217
-- Data for Name: avis; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 3435 (class 0 OID 172336)
-- Dependencies: 218
-- Data for Name: categories; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.categories VALUES (1, 'Entrée');
INSERT INTO public.categories VALUES (2, 'Plat');
INSERT INTO public.categories VALUES (3, 'Dessert');
INSERT INTO public.categories VALUES (4, 'Soupe');


--
-- TOC entry 3437 (class 0 OID 172340)
-- Dependencies: 220
-- Data for Name: categories_recettes; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.categories_recettes VALUES (1, 83);
INSERT INTO public.categories_recettes VALUES (4, 83);
INSERT INTO public.categories_recettes VALUES (2, 85);
INSERT INTO public.categories_recettes VALUES (4, 85);
INSERT INTO public.categories_recettes VALUES (1, 79);
INSERT INTO public.categories_recettes VALUES (2, 79);
INSERT INTO public.categories_recettes VALUES (4, 79);


--
-- TOC entry 3438 (class 0 OID 172343)
-- Dependencies: 221
-- Data for Name: etapes; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.etapes VALUES (85, 5, 'Finition', 'Retire le bouquet garni, rectifie l’assaisonnement et sers bien chaud avec un peu de persil frais haché.');
INSERT INTO public.etapes VALUES (83, 1, 'Préparer les ingrédients', 'Coupe la viande en gros cubes. Épluche et coupe les légumes. Émince l’oignon, l’ail et le poireau.');
INSERT INTO public.etapes VALUES (83, 2, 'Faire revenir la viande', 'Dans une grande cocotte, fais chauffer l’huile et fais revenir les morceaux de bœuf jusqu’à ce qu’ils soient bien dorés. Ajoute l’oignon et l’ail, laisse revenir 2 minutes.');
INSERT INTO public.etapes VALUES (83, 3, 'Ajouter les légumes', 'Ajoute tous les légumes coupés, le bouquet garni et le piment entier. Mélange bien.');
INSERT INTO public.etapes VALUES (83, 4, 'Cuisson lente', 'Verse l’eau, sale et poivre. Couvre et laisse mijoter à feu doux pendant environ 2 heures, jusqu’à ce que la viande soit fondante et les légumes bien cuits.');
INSERT INTO public.etapes VALUES (83, 5, 'Finalisation', 'Retire le piment et le bouquet garni. Ajuste l’assaisonnement et ajoute un filet de jus de citron vert avant de servir.');
INSERT INTO public.etapes VALUES (85, 1, 'Préparer les ingrédients', 'Épluche et coupe tous les légumes en morceaux. Coupe la viande en cubes.');
INSERT INTO public.etapes VALUES (85, 2, 'Faire revenir la viande', 'Dans une grande cocotte, fais chauffer l’huile d’olive et fais revenir les morceaux de bœuf jusqu’à ce qu’ils soient bien dorés. Ajoute l’oignon haché et l’ail écrasé, puis fais revenir 2 minutes.');
INSERT INTO public.etapes VALUES (85, 3, 'Ajouter les légumes', 'Ajoute les carottes, le poireau, le céleri, le navet, la tomate et les pommes de terre. Mélange bien pour enrober le tout des sucs de cuisson.');
INSERT INTO public.etapes VALUES (85, 4, 'Cuisson lente', 'Verse le bouillon ou l’eau, ajoute le bouquet garni, sale et poivre. Couvre et laisse mijoter à feu doux pendant environ 2 heures, jusqu’à ce que la viande soit tendre et les légumes fondants.');
INSERT INTO public.etapes VALUES (79, 1, 'Préparez le bouillon', 'Faites griller l’oignon et le gingembre (coupés en deux) directement à la flamme ou dans une poêle sans matière grasse jusqu’à ce qu’ils soient légèrement noircis. Cela apportera de la profondeur au bouillon.
Dans une grande marmite, mettez la viande, les épices (badiane, cannelle, clous de girofle, coriandre), l’oignon et le gingembre. Couvrez d’eau froide, portez à ébullition, puis écumez soigneusement.');
INSERT INTO public.etapes VALUES (79, 2, 'Laissez mijoter doucement', 'Réduisez le feu et laissez mijoter pendant environ 2 heures 30. Ajoutez le nuoc-mâm et le sucre roux à mi-cuisson. Le bouillon doit être clair et parfumé.');
INSERT INTO public.etapes VALUES (79, 3, 'Préparez les garnitures', 'Pendant que le bouillon cuit, faites tremper les nouilles de riz dans de l’eau chaude pendant environ 10 minutes, puis égouttez-les.
Coupez finement le bœuf cuit et préparez les herbes fraîches, le citron vert et les condiments pour le service.');
INSERT INTO public.etapes VALUES (79, 4, 'Assemblez la soupe', 'Répartissez les nouilles de riz dans des bols. Ajoutez les tranches de bœuf cuit et versez le bouillon bouillant par-dessus. La chaleur du bouillon réchauffera la viande.');
INSERT INTO public.etapes VALUES (79, 5, 'Terminez la présentation', 'Ajoutez des oignons nouveaux, des herbes fraîches (menthe, coriandre), un peu de piment selon votre goût et quelques gouttes de citron vert. Servez avec un peu de sauce hoisin à part.');


--
-- TOC entry 3439 (class 0 OID 172348)
-- Dependencies: 222
-- Data for Name: ingredients; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.ingredients VALUES (1, 'Viande de porc hachée');
INSERT INTO public.ingredients VALUES (2, 'Vermicelles de riz (ou de soja)');
INSERT INTO public.ingredients VALUES (3, 'Champignons noirs séchés');
INSERT INTO public.ingredients VALUES (4, 'Carotte râpée');
INSERT INTO public.ingredients VALUES (5, 'Oignon finement haché');
INSERT INTO public.ingredients VALUES (6, 'Gousses d''ail écrasées');
INSERT INTO public.ingredients VALUES (7, 'Oeuf ');
INSERT INTO public.ingredients VALUES (8, 'Cuillères à soupe de nuoc-mâm');
INSERT INTO public.ingredients VALUES (9, 'Sucre');
INSERT INTO public.ingredients VALUES (10, 'Poivre moulu');
INSERT INTO public.ingredients VALUES (11, 'Pousses de soja');
INSERT INTO public.ingredients VALUES (12, 'Galettes de riz');
INSERT INTO public.ingredients VALUES (13, 'Eau tiède légèrement sucrée');
INSERT INTO public.ingredients VALUES (14, 'Huile de friture');
INSERT INTO public.ingredients VALUES (15, 'de choucroute crue');
INSERT INTO public.ingredients VALUES (16, 'de lard fumé');
INSERT INTO public.ingredients VALUES (17, 'saucisses de Strasbourg');
INSERT INTO public.ingredients VALUES (18, 'saucisses Montbéliard');
INSERT INTO public.ingredients VALUES (19, 'jarret de porc demi-sel');
INSERT INTO public.ingredients VALUES (20, 'pommes de terre');
INSERT INTO public.ingredients VALUES (21, 'oignons');
INSERT INTO public.ingredients VALUES (22, 'escalopes de poulet');
INSERT INTO public.ingredients VALUES (23, 'd’huile d’olive');
INSERT INTO public.ingredients VALUES (24, 'de miel');
INSERT INTO public.ingredients VALUES (25, 'salade verte (mâche, roquette, laitue...)');
INSERT INTO public.ingredients VALUES (26, 'tomates cerises');
INSERT INTO public.ingredients VALUES (27, 'concombre');
INSERT INTO public.ingredients VALUES (28, 'd’huile d’olive
');
INSERT INTO public.ingredients VALUES (29, 'viande hachée (bœuf 15% MG de préférence)');
INSERT INTO public.ingredients VALUES (30, 'pains burger (briochés de préférence)

');
INSERT INTO public.ingredients VALUES (31, 'fromage cheddar');
INSERT INTO public.ingredients VALUES (32, 'salade verte');
INSERT INTO public.ingredients VALUES (33, 'tomate');
INSERT INTO public.ingredients VALUES (34, 'oignon rouge');
INSERT INTO public.ingredients VALUES (35, 'mayonnaise ou de sauce burger');
INSERT INTO public.ingredients VALUES (36, 'ketchup');
INSERT INTO public.ingredients VALUES (37, 'pâtes (spaghetti, penne ou tagliatelles)');
INSERT INTO public.ingredients VALUES (38, 'tomates bien mûres');
INSERT INTO public.ingredients VALUES (39, 'gousse d’ail');
INSERT INTO public.ingredients VALUES (40, 'd huile d’olive');
INSERT INTO public.ingredients VALUES (41, 'parmesan râpé');
INSERT INTO public.ingredients VALUES (42, 'bœuf (faux-filet ou rumsteck, finement tranché)');
INSERT INTO public.ingredients VALUES (43, 'nouilles de riz');
INSERT INTO public.ingredients VALUES (44, 'bouillon de bœuf');
INSERT INTO public.ingredients VALUES (45, 'oignon émincé');
INSERT INTO public.ingredients VALUES (46, 'd’ail hachée');
INSERT INTO public.ingredients VALUES (47, 'gingembre (2 cm), coupé en fines lamelles');
INSERT INTO public.ingredients VALUES (48, 'badiane (anis étoilé)');
INSERT INTO public.ingredients VALUES (49, 'cannelle');
INSERT INTO public.ingredients VALUES (50, 'sauce nuoc-mâm (ou sauce soja)');
INSERT INTO public.ingredients VALUES (51, 'sucre');
INSERT INTO public.ingredients VALUES (52, 'bœuf à braiser (paleron, gîte, macreuse) ');
INSERT INTO public.ingredients VALUES (53, 'carottes coupées en rondelles');
INSERT INTO public.ingredients VALUES (54, 'pommes de terre coupées en morceaux');
INSERT INTO public.ingredients VALUES (55, 'oignon ');
INSERT INTO public.ingredients VALUES (56, 'gousses d’ail hachées');
INSERT INTO public.ingredients VALUES (57, 'concentré de tomate');
INSERT INTO public.ingredients VALUES (58, '1 bouillon de bœuf');
INSERT INTO public.ingredients VALUES (176, 'crème liquide entière');
INSERT INTO public.ingredients VALUES (177, 'sachet de sucre vanillé');
INSERT INTO public.ingredients VALUES (178, 'jaunes d''œufs');
INSERT INTO public.ingredients VALUES (179, 'framboises (fraîches ou surgelées)');
INSERT INTO public.ingredients VALUES (180, 'sucre glace');
INSERT INTO public.ingredients VALUES (181, 'cuillère à soupe de jus de citron');
INSERT INTO public.ingredients VALUES (182, 'biscuits sablés écrasés');
INSERT INTO public.ingredients VALUES (183, 'chocolat noir pâtissier');
INSERT INTO public.ingredients VALUES (184, 'chocolat au lait');
INSERT INTO public.ingredients VALUES (185, 'beurre demi-sel');
INSERT INTO public.ingredients VALUES (186, 'crème liquide');
INSERT INTO public.ingredients VALUES (187, 'biscuits sablés émiettés');
INSERT INTO public.ingredients VALUES (188, 'biscuits digestifs ou sablés');
INSERT INTO public.ingredients VALUES (189, 'beurre fondu');
INSERT INTO public.ingredients VALUES (190, 'fromage frais (type Philadelphia)');
INSERT INTO public.ingredients VALUES (191, 'œufs');
INSERT INTO public.ingredients VALUES (192, 'citron (jus et zeste)');
INSERT INTO public.ingredients VALUES (193, 'farine');
INSERT INTO public.ingredients VALUES (194, 'chocolat noir');
INSERT INTO public.ingredients VALUES (195, 'pâte de noisette');
INSERT INTO public.ingredients VALUES (196, 'noisettes concassées');
INSERT INTO public.ingredients VALUES (197, 'feuille de laurier');
INSERT INTO public.ingredients VALUES (198, 'semoule de couscous');
INSERT INTO public.ingredients VALUES (199, 'viande (poulet ou agneau)');
INSERT INTO public.ingredients VALUES (200, 'carottes');
INSERT INTO public.ingredients VALUES (201, 'courgettes');
INSERT INTO public.ingredients VALUES (202, 'oignon');
INSERT INTO public.ingredients VALUES (203, 'boîte de pois chiches (200 g)');
INSERT INTO public.ingredients VALUES (204, 'huile d’olive');
INSERT INTO public.ingredients VALUES (205, 'Sel, poivre, ras-el-hanout');
INSERT INTO public.ingredients VALUES (206, 'lait');
INSERT INTO public.ingredients VALUES (207, 'cacao en poudre');
INSERT INTO public.ingredients VALUES (208, 'maïzena');
INSERT INTO public.ingredients VALUES (209, 'extrait de vanille');
INSERT INTO public.ingredients VALUES (210, 'bananes mûres');
INSERT INTO public.ingredients VALUES (211, 'biscuits secs');
INSERT INTO public.ingredients VALUES (212, 'beurre');
INSERT INTO public.ingredients VALUES (213, 'noix ou amandes (optionnel)');
INSERT INTO public.ingredients VALUES (214, 'cacao');
INSERT INTO public.ingredients VALUES (215, 'fruits rouges mélangés');
INSERT INTO public.ingredients VALUES (216, 'yaourt nature');
INSERT INTO public.ingredients VALUES (217, 'fruits frais');
INSERT INTO public.ingredients VALUES (218, 'miel');
INSERT INTO public.ingredients VALUES (219, 'graines de chia (optionnel)');
INSERT INTO public.ingredients VALUES (220, 'chocolat');
INSERT INTO public.ingredients VALUES (221, 'fruits rouges');
INSERT INTO public.ingredients VALUES (222, 'lasagnes');
INSERT INTO public.ingredients VALUES (223, 'viande hachée');
INSERT INTO public.ingredients VALUES (224, 'sauce tomate');
INSERT INTO public.ingredients VALUES (225, 'fromage râpé');
INSERT INTO public.ingredients VALUES (226, 'mascarpone');
INSERT INTO public.ingredients VALUES (227, 'crabe');
INSERT INTO public.ingredients VALUES (228, 'riz');
INSERT INTO public.ingredients VALUES (229, 'huile');
INSERT INTO public.ingredients VALUES (230, 'moules');
INSERT INTO public.ingredients VALUES (231, 'ail');
INSERT INTO public.ingredients VALUES (232, 'poulet');
INSERT INTO public.ingredients VALUES (233, 'crevettes');
INSERT INTO public.ingredients VALUES (234, 'petits pois');
INSERT INTO public.ingredients VALUES (235, 'poivron');
INSERT INTO public.ingredients VALUES (236, 'tomates');
INSERT INTO public.ingredients VALUES (237, 'olive');
INSERT INTO public.ingredients VALUES (238, 'bouillon');
INSERT INTO public.ingredients VALUES (239, 'pâte à pizza');
INSERT INTO public.ingredients VALUES (240, 'mozzarella');
INSERT INTO public.ingredients VALUES (241, 'basilic');
INSERT INTO public.ingredients VALUES (242, 'poulet entier');
INSERT INTO public.ingredients VALUES (243, 'citron');
INSERT INTO public.ingredients VALUES (244, 'Herbes de Provence');
INSERT INTO public.ingredients VALUES (245, 'filets de poulet');
INSERT INTO public.ingredients VALUES (246, 'safran');
INSERT INTO public.ingredients VALUES (247, 'bouillon de volaille');
INSERT INTO public.ingredients VALUES (248, 'queues de langouste');
INSERT INTO public.ingredients VALUES (249, 'jus de citron');
INSERT INTO public.ingredients VALUES (250, 'ail hachée');
INSERT INTO public.ingredients VALUES (251, 'paprika');
INSERT INTO public.ingredients VALUES (252, 'galettes de riz');
INSERT INTO public.ingredients VALUES (253, 'vermicelles de riz');
INSERT INTO public.ingredients VALUES (254, 'crevettes cuites');
INSERT INTO public.ingredients VALUES (255, 'carotte');
INSERT INTO public.ingredients VALUES (256, 'feuilles de laitue');
INSERT INTO public.ingredients VALUES (257, 'menthe fraîche');
INSERT INTO public.ingredients VALUES (258, 'coriandre');
INSERT INTO public.ingredients VALUES (259, 'Sauce pour nems ou sauce soja');
INSERT INTO public.ingredients VALUES (260, 'feuilles de brick');
INSERT INTO public.ingredients VALUES (261, 'bœuf haché');
INSERT INTO public.ingredients VALUES (262, 'carotte1');
INSERT INTO public.ingredients VALUES (263, 'curry');
INSERT INTO public.ingredients VALUES (264, 'fromage râpé (gruyère ou comté)');
INSERT INTO public.ingredients VALUES (265, 'riz à sushi');
INSERT INTO public.ingredients VALUES (266, 'eau');
INSERT INTO public.ingredients VALUES (267, 'vinaigre de riz');
INSERT INTO public.ingredients VALUES (268, 'sel');
INSERT INTO public.ingredients VALUES (269, 'saumon frais (qualité sashimi)');
INSERT INTO public.ingredients VALUES (270, 'avocat mûr');
INSERT INTO public.ingredients VALUES (271, 'feuilles de nori');
INSERT INTO public.ingredients VALUES (272, 'tortillas de blé');
INSERT INTO public.ingredients VALUES (273, 'poivron rouge');
INSERT INTO public.ingredients VALUES (274, 'cumin');
INSERT INTO public.ingredients VALUES (275, 'Sauce blanche ou sauce piquante');
INSERT INTO public.ingredients VALUES (276, 'pâte brisée');
INSERT INTO public.ingredients VALUES (277, 'crevettes décortiquées');
INSERT INTO public.ingredients VALUES (278, 'crème fraîche');
INSERT INTO public.ingredients VALUES (279, 'persil haché');
INSERT INTO public.ingredients VALUES (280, 'maïs en conserve');
INSERT INTO public.ingredients VALUES (281, 'haricots rouges en conserve');
INSERT INTO public.ingredients VALUES (282, 'tournedos de bœuf');
INSERT INTO public.ingredients VALUES (283, 'morilles fraîches ou réhydratées');
INSERT INTO public.ingredients VALUES (284, 'échalote');
INSERT INTO public.ingredients VALUES (285, 'betteraves cuites');
INSERT INTO public.ingredients VALUES (286, 'fromage frais');
INSERT INTO public.ingredients VALUES (287, 'ciboulette');
INSERT INTO public.ingredients VALUES (288, 'pavés de saumon');
INSERT INTO public.ingredients VALUES (289, 'persil');
INSERT INTO public.ingredients VALUES (290, 'blanc de poulet');
INSERT INTO public.ingredients VALUES (291, 'poireaux');
INSERT INTO public.ingredients VALUES (292, 'oignon

');
INSERT INTO public.ingredients VALUES (293, 'lardons fumés');
INSERT INTO public.ingredients VALUES (294, 'bouillon de volaille ou de légumes');
INSERT INTO public.ingredients VALUES (295, 'poivre');
INSERT INTO public.ingredients VALUES (296, 'framboises');
INSERT INTO public.ingredients VALUES (297, 'basilic frais');
INSERT INTO public.ingredients VALUES (298, 'gousse de vanille');
INSERT INTO public.ingredients VALUES (299, 'pâte sablée');
INSERT INTO public.ingredients VALUES (300, 'crème pâtissière');
INSERT INTO public.ingredients VALUES (301, 'fruits frais (fraises, kiwis, framboises…)');
INSERT INTO public.ingredients VALUES (302, 'lait de coco');
INSERT INTO public.ingredients VALUES (303, 'bouillon de poulet');
INSERT INTO public.ingredients VALUES (304, 'citrons verts');
INSERT INTO public.ingredients VALUES (305, 'citronnelle');
INSERT INTO public.ingredients VALUES (306, 'kaffir');
INSERT INTO public.ingredients VALUES (307, 'galanga');
INSERT INTO public.ingredients VALUES (308, 'piment rouge');
INSERT INTO public.ingredients VALUES (309, 'champignons de Paris');
INSERT INTO public.ingredients VALUES (310, 'sauce de poisson');
INSERT INTO public.ingredients VALUES (311, 'branches de céleri');
INSERT INTO public.ingredients VALUES (312, 'pâtes petites (type coquillettes)');
INSERT INTO public.ingredients VALUES (313, 'tomates concassées');
INSERT INTO public.ingredients VALUES (314, 'herbes de Provence');
INSERT INTO public.ingredients VALUES (315, 'bœuf à mijoter (paleron, jarret ou gîte)');
INSERT INTO public.ingredients VALUES (316, 'poireau');
INSERT INTO public.ingredients VALUES (317, 'giraumon (ou potiron)');
INSERT INTO public.ingredients VALUES (318, 'patate douce');
INSERT INTO public.ingredients VALUES (319, 'navet');
INSERT INTO public.ingredients VALUES (320, 'piment antillais entier');
INSERT INTO public.ingredients VALUES (321, 'bouquet garni (thym, persil, laurier)');
INSERT INTO public.ingredients VALUES (322, 'citron vert');
INSERT INTO public.ingredients VALUES (323, 'bœuf à mijoter (macreuse, jarret ou paleron)');
INSERT INTO public.ingredients VALUES (324, 'branche de céleri');
INSERT INTO public.ingredients VALUES (325, 'bouquet garni (thym, laurier, persil)');
INSERT INTO public.ingredients VALUES (326, 'eau ou de bouillon de bœuf');
INSERT INTO public.ingredients VALUES (327, 'bœuf (faux-filet ou entrecôte)');
INSERT INTO public.ingredients VALUES (328, 'bouillon de bœuf ou de dashi');
INSERT INTO public.ingredients VALUES (329, 'pâte miso (blanche ou rouge)');
INSERT INTO public.ingredients VALUES (330, 'sauce soja');
INSERT INTO public.ingredients VALUES (331, 'huile de sésame');
INSERT INTO public.ingredients VALUES (332, 'champignons shiitake émincés');
INSERT INTO public.ingredients VALUES (333, 'tofu ferme coupé en dés');
INSERT INTO public.ingredients VALUES (334, 'oignons nouveaux');
INSERT INTO public.ingredients VALUES (335, 'carotte coupée en fins bâtonnets');
INSERT INTO public.ingredients VALUES (336, 'nouilles soba ou udon

');
INSERT INTO public.ingredients VALUES (337, 'épinards frais');
INSERT INTO public.ingredients VALUES (338, 'jarret de bœuf');
INSERT INTO public.ingredients VALUES (339, 'plat de côte ou de poitrine de bœuf');
INSERT INTO public.ingredients VALUES (340, 'gingembre frais');
INSERT INTO public.ingredients VALUES (341, 'badiane');
INSERT INTO public.ingredients VALUES (342, 'girofle');
INSERT INTO public.ingredients VALUES (343, 'graines de coriandre');
INSERT INTO public.ingredients VALUES (344, 'sauce nuoc-mâm');
INSERT INTO public.ingredients VALUES (345, 'sucre roux');
INSERT INTO public.ingredients VALUES (346, 'nouilles de riz plates');
INSERT INTO public.ingredients VALUES (347, 'oignon nouveau');
INSERT INTO public.ingredients VALUES (348, 'coriandre fraîche');
INSERT INTO public.ingredients VALUES (349, 'banane');
INSERT INTO public.ingredients VALUES (350, 'pomme');
INSERT INTO public.ingredients VALUES (351, 'poire');
INSERT INTO public.ingredients VALUES (352, 'kiwi');
INSERT INTO public.ingredients VALUES (353, 'orange');
INSERT INTO public.ingredients VALUES (354, 'raisins');
INSERT INTO public.ingredients VALUES (355, 'ananas frais');


--
-- TOC entry 3441 (class 0 OID 172352)
-- Dependencies: 224
-- Data for Name: ingredients_recettes; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.ingredients_recettes VALUES (315, 83, '600 g de');
INSERT INTO public.ingredients_recettes VALUES (200, 83, '2');
INSERT INTO public.ingredients_recettes VALUES (316, 83, '1');
INSERT INTO public.ingredients_recettes VALUES (202, 83, '1');
INSERT INTO public.ingredients_recettes VALUES (231, 83, '2 gousses d’');
INSERT INTO public.ingredients_recettes VALUES (317, 83, '1 morceau de');
INSERT INTO public.ingredients_recettes VALUES (318, 83, '1');
INSERT INTO public.ingredients_recettes VALUES (319, 83, '1');
INSERT INTO public.ingredients_recettes VALUES (320, 83, '1');
INSERT INTO public.ingredients_recettes VALUES (321, 83, '1');
INSERT INTO public.ingredients_recettes VALUES (229, 83, '1 cuillère à soupe d’');
INSERT INTO public.ingredients_recettes VALUES (266, 83, '1,5 litre d’');
INSERT INTO public.ingredients_recettes VALUES (322, 83, '1');
INSERT INTO public.ingredients_recettes VALUES (323, 85, '600 g de');
INSERT INTO public.ingredients_recettes VALUES (200, 85, '2');
INSERT INTO public.ingredients_recettes VALUES (20, 85, '2');
INSERT INTO public.ingredients_recettes VALUES (316, 85, '1');
INSERT INTO public.ingredients_recettes VALUES (324, 85, '1');
INSERT INTO public.ingredients_recettes VALUES (319, 85, '1');
INSERT INTO public.ingredients_recettes VALUES (202, 85, '1');
INSERT INTO public.ingredients_recettes VALUES (231, 85, '2 gousses d’');
INSERT INTO public.ingredients_recettes VALUES (33, 85, '2');
INSERT INTO public.ingredients_recettes VALUES (325, 85, '1');
INSERT INTO public.ingredients_recettes VALUES (326, 85, '1,5 litre d');
INSERT INTO public.ingredients_recettes VALUES (204, 85, '1 cuillère à soupe d’');
INSERT INTO public.ingredients_recettes VALUES (338, 79, '500 g de');
INSERT INTO public.ingredients_recettes VALUES (339, 79, '250 g de');
INSERT INTO public.ingredients_recettes VALUES (202, 79, '1');
INSERT INTO public.ingredients_recettes VALUES (340, 79, '1 morceau de');
INSERT INTO public.ingredients_recettes VALUES (341, 79, '2 étoiles de');
INSERT INTO public.ingredients_recettes VALUES (49, 79, '1 bâton de');
INSERT INTO public.ingredients_recettes VALUES (342, 79, '3 clous de');
INSERT INTO public.ingredients_recettes VALUES (343, 79, '1 cuillère à soupe de');
INSERT INTO public.ingredients_recettes VALUES (266, 79, '2 litres d’');
INSERT INTO public.ingredients_recettes VALUES (344, 79, '2 cuillères à soupe de');
INSERT INTO public.ingredients_recettes VALUES (345, 79, '1 cuillère à soupe de');
INSERT INTO public.ingredients_recettes VALUES (346, 79, '250 g de');
INSERT INTO public.ingredients_recettes VALUES (347, 79, '1');
INSERT INTO public.ingredients_recettes VALUES (348, 79, '1 bouquet de');
INSERT INTO public.ingredients_recettes VALUES (322, 79, '1');
INSERT INTO public.ingredients_recettes VALUES (308, 79, '1');
INSERT INTO public.ingredients_recettes VALUES (257, 79, 'Quelques feuilles de');


--
-- TOC entry 3442 (class 0 OID 172355)
-- Dependencies: 225
-- Data for Name: recettes; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.recettes VALUES (83, 'Soupe antillaise', '00:30:00', '02:00:00', 3, '/images/recettes/c613e516-ee0e-4d79-8d3b-aee896650031.png', NULL);
INSERT INTO public.recettes VALUES (85, 'Soupe au boeuf et légumes', '00:25:00', '02:00:00', 2, '/images/recettes/7016568c-5bbc-4d7f-8db0-9e4083bf56e1.png', NULL);
INSERT INTO public.recettes VALUES (79, 'soupe vietnamienne', '00:30:00', '02:30:00', 3, '/images/recettes/2a1ed09a-8f08-4014-a15c-4a648350c16e.png', NULL);


--
-- TOC entry 3444 (class 0 OID 172361)
-- Dependencies: 227
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.roles VALUES (1, 'Administrateur');
INSERT INTO public.roles VALUES (2, 'Utilisateur');


--
-- TOC entry 3446 (class 0 OID 172365)
-- Dependencies: 229
-- Data for Name: utilisateurs; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.utilisateurs VALUES (1, 'orane111', 'orane111@live.fr', 'orane111', 1);
INSERT INTO public.utilisateurs VALUES (2, 'joss', 'joss@gmail.com', '$2a$11$AV9kH1Ktrfms4kEq9qgm1uRHI7MHbMu.BVUYJguDscJXq6WmSz.wi', 2);
INSERT INTO public.utilisateurs VALUES (3, 'fredon', 'fredon@gmail.com', '$2a$11$fthGLwdMj7BQL4QwE8uwOOPnLlkawjzl0eSqWcxD6th0poNvnsXL6', 1);
INSERT INTO public.utilisateurs VALUES (4, 'dorian', 'dorian@gmail.com', '$2a$11$vJXZ1TObU59mkLftxMer5uhDqv85rRxRcqFYBLU1szi2jS24D0zVi', 2);
INSERT INTO public.utilisateurs VALUES (5, 'Aline', 'aline@gmail.com', '$2a$11$9sxO2gkXhHPtVUw5LNGjmeJoUY1S3S3V.enUWCS04BItZq8jzQ9dO', 2);
INSERT INTO public.utilisateurs VALUES (6, 'Claire', 'claire@gmail.com', '$2a$11$6yUZ/L5Tx4eeEa.FA67JMevliJaoeYXaI4vd0CMkDGL2GQMp5LxD.', 2);
INSERT INTO public.utilisateurs VALUES (7, 'soraya', 'soraya@gmail.com', '$2a$11$WOWzx1gs37k7sxe0ZNvxc.JbwgTCmnsV1MxB2aCn65I6g/v8cFefO', 2);


--
-- TOC entry 3461 (class 0 OID 0)
-- Dependencies: 219
-- Name: categories_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.categories_id_seq', 43, true);


--
-- TOC entry 3462 (class 0 OID 0)
-- Dependencies: 223
-- Name: ingredients_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredients_id_seq', 355, true);


--
-- TOC entry 3463 (class 0 OID 0)
-- Dependencies: 226
-- Name: recettes_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.recettes_id_seq', 89, true);


--
-- TOC entry 3464 (class 0 OID 0)
-- Dependencies: 228
-- Name: roles_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.roles_id_seq', 1, false);


--
-- TOC entry 3465 (class 0 OID 0)
-- Dependencies: 230
-- Name: utilisateurs_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.utilisateurs_id_seq', 7, true);


--
-- TOC entry 3255 (class 2606 OID 172375)
-- Name: avis avis_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.avis
    ADD CONSTRAINT avis_pkey PRIMARY KEY (id_recette, id_utilisateur);


--
-- TOC entry 3257 (class 2606 OID 172377)
-- Name: categories categories_nom_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categories
    ADD CONSTRAINT categories_nom_key UNIQUE (nom);


--
-- TOC entry 3259 (class 2606 OID 172379)
-- Name: categories categories_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categories
    ADD CONSTRAINT categories_pkey PRIMARY KEY (id);


--
-- TOC entry 3261 (class 2606 OID 172381)
-- Name: categories_recettes categories_recettes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categories_recettes
    ADD CONSTRAINT categories_recettes_pkey PRIMARY KEY (id_categorie, id_recette);


--
-- TOC entry 3263 (class 2606 OID 172383)
-- Name: etapes etapes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.etapes
    ADD CONSTRAINT etapes_pkey PRIMARY KEY (id_recette, numero);


--
-- TOC entry 3265 (class 2606 OID 172385)
-- Name: ingredients ingredients_nom_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients
    ADD CONSTRAINT ingredients_nom_key UNIQUE (nom);


--
-- TOC entry 3267 (class 2606 OID 172387)
-- Name: ingredients ingredients_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients
    ADD CONSTRAINT ingredients_pkey PRIMARY KEY (id);


--
-- TOC entry 3269 (class 2606 OID 172389)
-- Name: ingredients_recettes ingredients_recettes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients_recettes
    ADD CONSTRAINT ingredients_recettes_pkey PRIMARY KEY (id_ingredient, id_recette);


--
-- TOC entry 3271 (class 2606 OID 172391)
-- Name: recettes recettes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recettes
    ADD CONSTRAINT recettes_pkey PRIMARY KEY (id);


--
-- TOC entry 3273 (class 2606 OID 172393)
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);


--
-- TOC entry 3275 (class 2606 OID 172395)
-- Name: utilisateurs utilisateurs_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.utilisateurs
    ADD CONSTRAINT utilisateurs_email_key UNIQUE (email);


--
-- TOC entry 3277 (class 2606 OID 172397)
-- Name: utilisateurs utilisateurs_identifiant_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.utilisateurs
    ADD CONSTRAINT utilisateurs_identifiant_key UNIQUE (identifiant);


--
-- TOC entry 3279 (class 2606 OID 172399)
-- Name: utilisateurs utilisateurs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.utilisateurs
    ADD CONSTRAINT utilisateurs_pkey PRIMARY KEY (id);


--
-- TOC entry 3280 (class 2606 OID 172400)
-- Name: avis fk_avis_recette; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.avis
    ADD CONSTRAINT fk_avis_recette FOREIGN KEY (id_recette) REFERENCES public.recettes(id) ON DELETE CASCADE;


--
-- TOC entry 3281 (class 2606 OID 172405)
-- Name: avis fk_avis_utilisateurs; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.avis
    ADD CONSTRAINT fk_avis_utilisateurs FOREIGN KEY (id_utilisateur) REFERENCES public.utilisateurs(id);


--
-- TOC entry 3282 (class 2606 OID 172410)
-- Name: categories_recettes fk_categories_recettes_categorie; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categories_recettes
    ADD CONSTRAINT fk_categories_recettes_categorie FOREIGN KEY (id_categorie) REFERENCES public.categories(id) ON DELETE CASCADE;


--
-- TOC entry 3283 (class 2606 OID 172415)
-- Name: categories_recettes fk_categories_recettes_recette; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categories_recettes
    ADD CONSTRAINT fk_categories_recettes_recette FOREIGN KEY (id_recette) REFERENCES public.recettes(id) ON DELETE CASCADE;


--
-- TOC entry 3284 (class 2606 OID 172420)
-- Name: etapes fk_etapes_recette; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.etapes
    ADD CONSTRAINT fk_etapes_recette FOREIGN KEY (id_recette) REFERENCES public.recettes(id) ON DELETE CASCADE;


--
-- TOC entry 3285 (class 2606 OID 172425)
-- Name: ingredients_recettes fk_ingredients_recettes_ingredient; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients_recettes
    ADD CONSTRAINT fk_ingredients_recettes_ingredient FOREIGN KEY (id_ingredient) REFERENCES public.ingredients(id) ON DELETE CASCADE;


--
-- TOC entry 3286 (class 2606 OID 172430)
-- Name: ingredients_recettes fk_ingredients_recettes_recette; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients_recettes
    ADD CONSTRAINT fk_ingredients_recettes_recette FOREIGN KEY (id_recette) REFERENCES public.recettes(id) ON DELETE CASCADE;


--
-- TOC entry 3287 (class 2606 OID 172435)
-- Name: recettes fk_recettes_utilisateurs; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recettes
    ADD CONSTRAINT fk_recettes_utilisateurs FOREIGN KEY (id_utilisateur) REFERENCES public.utilisateurs(id);


--
-- TOC entry 3288 (class 2606 OID 172440)
-- Name: utilisateurs utilisateurs_role_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.utilisateurs
    ADD CONSTRAINT utilisateurs_role_id_fkey FOREIGN KEY (role_id) REFERENCES public.roles(id);


--
-- TOC entry 3455 (class 0 OID 0)
-- Dependencies: 5
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;


-- Completed on 2025-10-23 08:53:36

--
-- PostgreSQL database dump complete
--